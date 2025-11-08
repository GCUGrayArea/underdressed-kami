using Microsoft.Extensions.Logging;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Infrastructure.ExternalServices.Models;

namespace SmartScheduler.Infrastructure.ExternalServices;

/// <summary>
/// Main service orchestrating distance calculations via cache, API, and fallback.
/// Three-layer approach: Cache → API → Fallback (Haversine formula).
/// </summary>
public sealed class DistanceCalculator : IDistanceCalculator
{
    private readonly OpenRouteServiceClient _apiClient;
    private readonly DistanceCache _cache;
    private readonly ILogger<DistanceCalculator> _logger;

    public DistanceCalculator(
        OpenRouteServiceClient apiClient,
        DistanceCache cache,
        ILogger<DistanceCalculator> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculates driving distance and travel time between two locations.
    /// Strategy: Cache → API → Fallback (straight-line distance).
    /// </summary>
    public async Task<DistanceResult> CalculateDistanceAsync(
        Location origin,
        Location destination,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Check cache
        var cacheKey = _cache.GenerateCacheKey(
            origin.Latitude, origin.Longitude,
            destination.Latitude, destination.Longitude);

        if (_cache.TryGetCached(cacheKey, out var cachedResult) && cachedResult != null)
        {
            return cachedResult;
        }

        // Step 2: Try API call
        var apiResult = await TryGetFromApiAsync(origin, destination, cancellationToken);
        if (apiResult != null)
        {
            _cache.CacheResult(cacheKey, apiResult);
            return apiResult;
        }

        // Step 3: Fallback to straight-line distance
        return CalculateFallbackDistance(origin, destination);
    }

    /// <summary>
    /// Attempts to get distance from OpenRouteService API.
    /// Returns null if API call fails.
    /// </summary>
    private async Task<DistanceResult?> TryGetFromApiAsync(
        Location origin,
        Location destination,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = RouteRequest.Create(
                origin.Latitude, origin.Longitude,
                destination.Latitude, destination.Longitude);

            var response = await _apiClient.GetRouteAsync(request, cancellationToken);

            if (response == null)
            {
                _logger.LogWarning("API call failed, falling back to straight-line distance");
                return null;
            }

            return ConvertApiResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling OpenRouteService API");
            return null;
        }
    }

    /// <summary>
    /// Converts OpenRouteService API response to DistanceResult.
    /// </summary>
    private DistanceResult ConvertApiResponse(RouteResponse response)
    {
        var distanceMeters = response.GetDistanceMeters();
        var durationSeconds = response.GetDurationSeconds();

        // Convert to miles and minutes
        var distanceMiles = distanceMeters * 0.000621371; // meters to miles
        var durationMinutes = durationSeconds / 60.0;      // seconds to minutes

        return new DistanceResult(
            distanceMiles,
            durationMinutes,
            DistanceSource.Api,
            "Calculated via OpenRouteService API");
    }

    /// <summary>
    /// Calculates straight-line distance using Haversine formula.
    /// Used as fallback when API is unavailable.
    /// </summary>
    private DistanceResult CalculateFallbackDistance(Location origin, Location destination)
    {
        var straightLineDistanceMiles = origin.DistanceToMiles(destination);

        _logger.LogInformation(
            "Using fallback distance calculation: {Distance:F2} miles straight-line",
            straightLineDistanceMiles);

        return DistanceResult.CreateFallback(
            straightLineDistanceMiles,
            "API unavailable, using straight-line distance");
    }
}

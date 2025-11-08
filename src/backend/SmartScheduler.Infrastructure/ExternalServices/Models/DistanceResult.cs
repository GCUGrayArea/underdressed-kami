namespace SmartScheduler.Infrastructure.ExternalServices.Models;

/// <summary>
/// Result of a distance calculation containing distance, duration, and metadata.
/// Indicates whether result came from API, cache, or fallback calculation.
/// </summary>
public sealed record DistanceResult
{
    /// <summary>
    /// Driving distance in miles.
    /// </summary>
    public double DistanceMiles { get; init; }

    /// <summary>
    /// Estimated driving duration in minutes.
    /// For fallback calculations, estimated as distance / 40mph average speed.
    /// </summary>
    public double DurationMinutes { get; init; }

    /// <summary>
    /// Source of the distance calculation result.
    /// </summary>
    public DistanceSource Source { get; init; }

    /// <summary>
    /// Additional context or error information if applicable.
    /// </summary>
    public string? Message { get; init; }

    public DistanceResult(
        double distanceMiles,
        double durationMinutes,
        DistanceSource source,
        string? message = null)
    {
        if (distanceMiles < 0)
            throw new ArgumentOutOfRangeException(
                nameof(distanceMiles),
                "Distance cannot be negative");

        if (durationMinutes < 0)
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                "Duration cannot be negative");

        DistanceMiles = distanceMiles;
        DurationMinutes = durationMinutes;
        Source = source;
        Message = message;
    }

    /// <summary>
    /// Creates a fallback result using straight-line distance.
    /// </summary>
    public static DistanceResult CreateFallback(double distanceMiles, string? reason = null)
    {
        // Estimate duration: assume 40mph average speed for straight-line distance
        var estimatedDurationMinutes = (distanceMiles / 40.0) * 60.0;

        return new DistanceResult(
            distanceMiles,
            estimatedDurationMinutes,
            DistanceSource.Fallback,
            reason ?? "Using straight-line distance calculation");
    }
}

/// <summary>
/// Indicates the source of a distance calculation.
/// </summary>
public enum DistanceSource
{
    /// <summary>
    /// Result retrieved from OpenRouteService API.
    /// </summary>
    Api,

    /// <summary>
    /// Result retrieved from cache (previously fetched from API).
    /// </summary>
    Cache,

    /// <summary>
    /// Result calculated using straight-line distance (Haversine formula).
    /// Used when API is unavailable or returns an error.
    /// </summary>
    Fallback
}

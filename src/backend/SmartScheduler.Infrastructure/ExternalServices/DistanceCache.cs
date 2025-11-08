using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartScheduler.Infrastructure.ExternalServices.Models;

namespace SmartScheduler.Infrastructure.ExternalServices;

/// <summary>
/// In-memory cache for distance calculation results.
/// Caches successful API responses for 24 hours to reduce API calls.
/// Thread-safe implementation supporting concurrent access.
/// </summary>
public sealed class DistanceCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<DistanceCache> _logger;

    // Cache configuration
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);
    private const int MaxCacheEntries = 10000;

    public DistanceCache(IMemoryCache cache, ILogger<DistanceCache> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates bidirectional cache key from two locations.
    /// Key is same regardless of origin/destination order for efficiency.
    /// Format: "{lat1:F6},{lon1:F6}|{lat2:F6},{lon2:F6}"
    /// </summary>
    public string GenerateCacheKey(double lat1, double lon1, double lat2, double lon2)
    {
        // Normalize key order to ensure bidirectionality
        // (A→B and B→A use same cache entry)
        if (lat1 > lat2 || (lat1 == lat2 && lon1 > lon2))
        {
            // Swap to ensure consistent ordering
            (lat1, lat2) = (lat2, lat1);
            (lon1, lon2) = (lon2, lon1);
        }

        return $"{lat1:F6},{lon1:F6}|{lat2:F6},{lon2:F6}";
    }

    /// <summary>
    /// Attempts to retrieve cached distance result.
    /// Returns true if found, false if not in cache or expired.
    /// </summary>
    public bool TryGetCached(string cacheKey, out DistanceResult? result)
    {
        if (_cache.TryGetValue(cacheKey, out DistanceResult? cachedResult) && cachedResult != null)
        {
            _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);

            // Mark result as from cache
            result = cachedResult with { Source = DistanceSource.Cache };
            return true;
        }

        _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);
        result = null;
        return false;
    }

    /// <summary>
    /// Stores distance result in cache with 24-hour expiration.
    /// Only caches successful API results (not fallback calculations).
    /// </summary>
    public void CacheResult(string cacheKey, DistanceResult result)
    {
        // Only cache API results, not fallback calculations
        if (result.Source != DistanceSource.Api)
        {
            _logger.LogDebug(
                "Skipping cache for non-API result (source: {Source})",
                result.Source);
            return;
        }

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration,
            Size = 1 // Each entry counts as 1 unit toward size limit
        };

        _cache.Set(cacheKey, result, cacheOptions);

        _logger.LogInformation(
            "Cached distance result for key: {CacheKey} (expires in {Hours}h)",
            cacheKey,
            CacheDuration.TotalHours);
    }
}

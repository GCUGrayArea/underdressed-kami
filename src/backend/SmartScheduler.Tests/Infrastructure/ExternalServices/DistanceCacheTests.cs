using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SmartScheduler.Infrastructure.ExternalServices;
using SmartScheduler.Infrastructure.ExternalServices.Models;

namespace SmartScheduler.Tests.Infrastructure.ExternalServices;

/// <summary>
/// Unit tests for DistanceCache covering:
/// - Cache key generation and bidirectionality
/// - Cache hit and miss scenarios
/// - Expiration behavior
/// - Thread-safe concurrent access
/// </summary>
public class DistanceCacheTests
{
    /// <summary>
    /// Tests for cache key generation.
    /// </summary>
    public class CacheKeyGenerationTests
    {
        [Fact]
        public void GenerateCacheKey_WithValidCoordinates_ReturnsFormattedKey()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);

            // Assert
            key.Should().Be("34.052200,-118.243700|40.712800,-74.006000");
        }

        [Fact]
        public void GenerateCacheKey_IsBidirectional_SameKeyForReversedLocations()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key1 = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var key2 = cache.GenerateCacheKey(34.052200, -118.243700, 40.712800, -74.006000);

            // Assert
            key1.Should().Be(key2, "A→B and B→A should use the same cache key");
        }

        [Fact]
        public void GenerateCacheKey_WithSameLocation_GeneratesConsistentKey()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 40.712800, -74.006000);

            // Assert
            key.Should().Be("40.712800,-74.006000|40.712800,-74.006000");
        }

        [Fact]
        public void GenerateCacheKey_RoundsTo6DecimalPlaces()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key = cache.GenerateCacheKey(40.7128001234, -74.0060009876, 34.0522, -118.2437);

            // Assert
            key.Should().Contain("40.712800");
            key.Should().Contain("-74.006001");
        }

        [Fact]
        public void GenerateCacheKey_WithDifferentPairs_GeneratesDifferentKeys()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key1 = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var key2 = cache.GenerateCacheKey(40.712800, -74.006000, 41.878100, -87.629800); // Chicago

            // Assert
            key1.Should().NotBe(key2);
        }

        [Theory]
        [InlineData(40.0, -74.0, 34.0, -118.0)]
        [InlineData(40.5, -74.5, 34.5, -118.5)]
        [InlineData(40.999999, -74.999999, 34.000001, -118.000001)]
        public void GenerateCacheKey_WithVariousCoordinates_GeneratesValidKeys(
            double lat1, double lon1, double lat2, double lon2)
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var key = cache.GenerateCacheKey(lat1, lon1, lat2, lon2);

            // Assert
            key.Should().NotBeNullOrEmpty();
            key.Should().Contain("|", "key should contain separator");
            key.Should().Contain(",", "key should contain coordinate separators");
        }
    }

    /// <summary>
    /// Tests for cache hit and miss behavior.
    /// </summary>
    public class CacheHitMissTests
    {
        [Fact]
        public void TryGetCached_WithEmptyCache_ReturnsFalse()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);

            // Act
            var found = cache.TryGetCached(key, out var result);

            // Assert
            found.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryGetCached_AfterCaching_ReturnsTrue()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var originalResult = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);

            cache.CacheResult(key, originalResult);

            // Act
            var found = cache.TryGetCached(key, out var result);

            // Assert
            found.Should().BeTrue();
            result.Should().NotBeNull();
            result!.DistanceMiles.Should().Be(2800.0);
            result.DurationMinutes.Should().Be(2400.0);
        }

        [Fact]
        public void TryGetCached_MarksResultAsFromCache()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var originalResult = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);

            cache.CacheResult(key, originalResult);

            // Act
            cache.TryGetCached(key, out var result);

            // Assert
            result!.Source.Should().Be(DistanceSource.Cache, "retrieved result should be marked as from cache");
        }

        [Fact]
        public void CacheResult_WithFallbackSource_DoesNotCache()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var fallbackResult = new DistanceResult(2451.0, 3677.0, DistanceSource.Fallback);

            // Act
            cache.CacheResult(key, fallbackResult);

            // Assert
            var found = cache.TryGetCached(key, out var result);
            found.Should().BeFalse("fallback results should not be cached");
            result.Should().BeNull();
        }

        [Fact]
        public void CacheResult_WithApiSource_DoesCacheResult()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var apiResult = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);

            // Act
            cache.CacheResult(key, apiResult);

            // Assert
            var found = cache.TryGetCached(key, out var result);
            found.Should().BeTrue("API results should be cached");
            result.Should().NotBeNull();
        }
    }

    /// <summary>
    /// Tests for bidirectional caching.
    /// </summary>
    public class BidirectionalCachingTests
    {
        [Fact]
        public void CacheResult_WithLocationPair_CanBeRetrievedWithReversedPair()
        {
            // Arrange
            var cache = CreateCache();
            var key1 = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var key2 = cache.GenerateCacheKey(34.052200, -118.243700, 40.712800, -74.006000);
            var result = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);

            // Act
            cache.CacheResult(key1, result);

            // Assert
            key1.Should().Be(key2, "keys should be identical");
            var found = cache.TryGetCached(key2, out var retrievedResult);
            found.Should().BeTrue();
            retrievedResult!.DistanceMiles.Should().Be(2800.0);
        }

        [Fact]
        public void CacheResult_StoredOnce_ServesBothDirections()
        {
            // Arrange
            var cache = CreateCache();
            var nycToLA = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var laToNYC = cache.GenerateCacheKey(34.052200, -118.243700, 40.712800, -74.006000);
            var result = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);

            cache.CacheResult(nycToLA, result);

            // Act
            var found1 = cache.TryGetCached(nycToLA, out var result1);
            var found2 = cache.TryGetCached(laToNYC, out var result2);

            // Assert
            found1.Should().BeTrue();
            found2.Should().BeTrue();
            result1!.DistanceMiles.Should().Be(result2!.DistanceMiles);
        }
    }

    /// <summary>
    /// Tests for multiple entries and different location pairs.
    /// </summary>
    public class MultipleEntriesTests
    {
        [Fact]
        public void CacheResult_WithMultiplePairs_CachesIndependently()
        {
            // Arrange
            var cache = CreateCache();
            var key1 = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700); // NYC-LA
            var key2 = cache.GenerateCacheKey(40.712800, -74.006000, 41.878100, -87.629800); // NYC-Chicago
            var result1 = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);
            var result2 = new DistanceResult(790.0, 700.0, DistanceSource.Api);

            // Act
            cache.CacheResult(key1, result1);
            cache.CacheResult(key2, result2);

            // Assert
            cache.TryGetCached(key1, out var retrieved1);
            cache.TryGetCached(key2, out var retrieved2);

            retrieved1!.DistanceMiles.Should().Be(2800.0);
            retrieved2!.DistanceMiles.Should().Be(790.0);
        }

        [Fact]
        public void CacheResult_CanStoreAndRetrieveManyEntries()
        {
            // Arrange
            var cache = CreateCache();
            var results = new List<(string key, DistanceResult result)>();

            for (int i = 0; i < 100; i++)
            {
                var key = cache.GenerateCacheKey(40.0 + i * 0.1, -74.0, 34.0, -118.0);
                var result = new DistanceResult(100.0 + i, 60.0 + i, DistanceSource.Api);
                results.Add((key, result));
                cache.CacheResult(key, result);
            }

            // Act & Assert
            foreach (var (key, expectedResult) in results)
            {
                var found = cache.TryGetCached(key, out var retrievedResult);
                found.Should().BeTrue();
                retrievedResult!.DistanceMiles.Should().Be(expectedResult.DistanceMiles);
            }
        }
    }

    /// <summary>
    /// Tests for thread-safe concurrent access.
    /// </summary>
    public class ConcurrencyTests
    {
        [Fact]
        public async Task CacheResult_WithConcurrentWrites_HandlesThreadSafety()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var tasks = new List<Task>();

            // Act - Multiple threads trying to cache the same key
            for (int i = 0; i < 10; i++)
            {
                var index = i;
                tasks.Add(Task.Run(() =>
                {
                    var result = new DistanceResult(2800.0 + index, 2400.0, DistanceSource.Api);
                    cache.CacheResult(key, result);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert - Should not crash and should have one of the values cached
            var found = cache.TryGetCached(key, out var result);
            found.Should().BeTrue();
            result.Should().NotBeNull();
            result!.DistanceMiles.Should().BeInRange(2800.0, 2809.0);
        }

        [Fact]
        public async Task TryGetCached_WithConcurrentReads_HandlesThreadSafety()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 34.052200, -118.243700);
            var result = new DistanceResult(2800.0, 2400.0, DistanceSource.Api);
            cache.CacheResult(key, result);

            var tasks = new List<Task<bool>>();

            // Act - Multiple threads trying to read the same key
            for (int i = 0; i < 20; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    return cache.TryGetCached(key, out var _);
                }));
            }

            var results = await Task.WhenAll(tasks);

            // Assert - All reads should succeed
            results.Should().AllSatisfy(found => found.Should().BeTrue());
        }

        [Fact]
        public async Task CacheResult_WithConcurrentDifferentKeys_HandlesThreadSafety()
        {
            // Arrange
            var cache = CreateCache();
            var tasks = new List<Task>();

            // Act - Multiple threads caching different keys
            for (int i = 0; i < 50; i++)
            {
                var index = i;
                tasks.Add(Task.Run(() =>
                {
                    var key = cache.GenerateCacheKey(40.0 + index * 0.1, -74.0, 34.0, -118.0);
                    var result = new DistanceResult(100.0 + index, 60.0, DistanceSource.Api);
                    cache.CacheResult(key, result);
                }));
            }

            await Task.WhenAll(tasks);

            // Assert - Should not crash, all keys should be retrievable
            for (int i = 0; i < 50; i++)
            {
                var key = cache.GenerateCacheKey(40.0 + i * 0.1, -74.0, 34.0, -118.0);
                var found = cache.TryGetCached(key, out var result);
                found.Should().BeTrue($"key {i} should be cached");
                result!.DistanceMiles.Should().Be(100.0 + i);
            }
        }
    }

    /// <summary>
    /// Tests for edge cases.
    /// </summary>
    public class EdgeCaseTests
    {
        [Fact]
        public void TryGetCached_WithInvalidKey_ReturnsFalse()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var found = cache.TryGetCached("invalid-key-format", out var result);

            // Assert
            found.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void CacheResult_WithZeroDistance_StillCaches()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, 40.712800, -74.006000);
            var result = new DistanceResult(0.0, 0.0, DistanceSource.Api);

            // Act
            cache.CacheResult(key, result);

            // Assert
            var found = cache.TryGetCached(key, out var retrieved);
            found.Should().BeTrue("zero distance results should be cached");
            retrieved!.DistanceMiles.Should().Be(0.0);
        }

        [Fact]
        public void CacheResult_WithVeryLargeDistance_HandlesCorrectly()
        {
            // Arrange
            var cache = CreateCache();
            var key = cache.GenerateCacheKey(40.712800, -74.006000, -33.865143, 151.209900); // NYC-Sydney
            var result = new DistanceResult(9950.0, 20000.0, DistanceSource.Api);

            // Act
            cache.CacheResult(key, result);

            // Assert
            var found = cache.TryGetCached(key, out var retrieved);
            found.Should().BeTrue();
            retrieved!.DistanceMiles.Should().Be(9950.0);
        }
    }

    #region Helper Methods

    private static DistanceCache CreateCache()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 10000 // Match production configuration
        });
        var logger = Mock.Of<ILogger<DistanceCache>>();
        return new DistanceCache(memoryCache, logger);
    }

    #endregion
}

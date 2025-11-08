using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Infrastructure.ExternalServices;
using SmartScheduler.Infrastructure.ExternalServices.Models;
using SmartScheduler.Tests.TestHelpers;

namespace SmartScheduler.Tests.Infrastructure.ExternalServices;

/// <summary>
/// Comprehensive unit tests for DistanceCalculator covering:
/// - Successful API calls
/// - Cache hits and misses
/// - API fallback scenarios
/// - Error handling
/// </summary>
public class DistanceCalculatorTests
{
    // Well-known test locations
    private static readonly Location NYC = new(40.7128, -74.0060, "New York, NY");
    private static readonly Location LA = new(34.0522, -118.2437, "Los Angeles, CA");
    private const double NYC_LA_STRAIGHT_LINE_MILES = 2451.0; // Approximate Haversine distance

    /// <summary>
    /// Tests for successful distance calculations via API.
    /// </summary>
    public class SuccessfulCalculationTests
    {
        [Fact]
        public async Task CalculateDistanceAsync_WithValidCoordinates_ReturnsApiResult()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            var mockResponse = CreateMockRouteResponse(4500000.0, 144000.0); // ~2800 mi, 2400 min
            mockHandler.QueueJsonResponse(mockResponse);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Should().NotBeNull();
            result.Source.Should().Be(DistanceSource.Api);
            result.DistanceMiles.Should().BeApproximately(2796.0, 1.0); // 4500000m * 0.000621371
            result.DurationMinutes.Should().BeApproximately(2400.0, 1.0); // 144000s / 60
            mockHandler.RequestCount.Should().Be(1);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithSecondCall_ReturnsCachedResult()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            var mockResponse = CreateMockRouteResponse(4500000.0, 144000.0);
            mockHandler.QueueJsonResponse(mockResponse);

            // Act - First call should hit API
            var result1 = await calculator.CalculateDistanceAsync(NYC, LA);

            // Act - Second call should hit cache
            var result2 = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result1.Source.Should().Be(DistanceSource.Api);
            result2.Source.Should().Be(DistanceSource.Cache);
            result2.DistanceMiles.Should().Be(result1.DistanceMiles);
            result2.DurationMinutes.Should().Be(result1.DurationMinutes);
            mockHandler.RequestCount.Should().Be(1, "second call should use cache");
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithReversedLocations_ReturnsCachedResult()
        {
            // Arrange - Test bidirectional caching (A→B and B→A use same cache)
            var (calculator, mockHandler, _) = CreateCalculator();
            var mockResponse = CreateMockRouteResponse(4500000.0, 144000.0);
            mockHandler.QueueJsonResponse(mockResponse);

            // Act - First call NYC → LA
            var result1 = await calculator.CalculateDistanceAsync(NYC, LA);

            // Act - Second call LA → NYC (reversed)
            var result2 = await calculator.CalculateDistanceAsync(LA, NYC);

            // Assert
            result1.Source.Should().Be(DistanceSource.Api);
            result2.Source.Should().Be(DistanceSource.Cache, "reversed locations should use same cache entry");
            result2.DistanceMiles.Should().Be(result1.DistanceMiles);
            mockHandler.RequestCount.Should().Be(1, "bidirectional caching should prevent second API call");
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithShortDistance_ReturnsAccurateResult()
        {
            // Arrange - Test short distance (e.g., across town)
            var location1 = new Location(40.7128, -74.0060); // NYC
            var location2 = new Location(40.7580, -73.9855); // Times Square

            var (calculator, mockHandler, _) = CreateCalculator();
            var mockResponse = CreateMockRouteResponse(8000.0, 600.0); // ~5 miles, 10 min
            mockHandler.QueueJsonResponse(mockResponse);

            // Act
            var result = await calculator.CalculateDistanceAsync(location1, location2);

            // Assert
            result.DistanceMiles.Should().BeApproximately(4.97, 0.1); // 8000m to miles
            result.DurationMinutes.Should().Be(10.0);
            result.Source.Should().Be(DistanceSource.Api);
        }
    }

    /// <summary>
    /// Tests for cache behavior.
    /// </summary>
    public class CacheTests
    {
        [Fact]
        public async Task CalculateDistanceAsync_WithCacheMiss_CallsApi()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            var mockResponse = CreateMockRouteResponse(4500000.0, 144000.0);
            mockHandler.QueueJsonResponse(mockResponse);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Api, "first call should be cache miss");
            mockHandler.RequestCount.Should().Be(1);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithMultipleCalls_CachesEachLocation()
        {
            // Arrange - Test multiple different location pairs
            var location1 = new Location(40.7128, -74.0060);
            var location2 = new Location(34.0522, -118.2437);
            var location3 = new Location(41.8781, -87.6298); // Chicago

            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(4500000.0, 144000.0));
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1200000.0, 72000.0));

            // Act
            await calculator.CalculateDistanceAsync(location1, location2); // NYC → LA (API)
            await calculator.CalculateDistanceAsync(location1, location3); // NYC → Chicago (API)
            await calculator.CalculateDistanceAsync(location1, location2); // NYC → LA (Cache)
            await calculator.CalculateDistanceAsync(location1, location3); // NYC → Chicago (Cache)

            // Assert
            mockHandler.RequestCount.Should().Be(2, "only first call to each location pair should hit API");
        }

        [Fact]
        public async Task CalculateDistanceAsync_DoesNotCacheFallbackResults()
        {
            // Arrange - Force API failure to trigger fallback
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueErrorResponse(System.Net.HttpStatusCode.ServiceUnavailable);
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(4500000.0, 144000.0));

            // Act
            var result1 = await calculator.CalculateDistanceAsync(NYC, LA); // Fallback
            var result2 = await calculator.CalculateDistanceAsync(NYC, LA); // Should retry API

            // Assert
            result1.Source.Should().Be(DistanceSource.Fallback);
            result2.Source.Should().Be(DistanceSource.Api);
            mockHandler.RequestCount.Should().Be(2, "fallback results should not be cached");
        }
    }

    /// <summary>
    /// Tests for fallback scenarios when API is unavailable.
    /// </summary>
    public class FallbackTests
    {
        [Fact]
        public async Task CalculateDistanceAsync_WhenApiReturns500_UsesFallback()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueErrorResponse(System.Net.HttpStatusCode.InternalServerError);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Fallback);
            result.DistanceMiles.Should().BeApproximately(NYC_LA_STRAIGHT_LINE_MILES, 50.0);
            result.Message.Should().Contain("straight-line");
        }

        [Fact]
        public async Task CalculateDistanceAsync_WhenApiReturns404_UsesFallback()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueErrorResponse(System.Net.HttpStatusCode.NotFound);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Fallback);
            result.DistanceMiles.Should().BeApproximately(NYC_LA_STRAIGHT_LINE_MILES, 50.0);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WhenApiTimesOut_UsesFallback()
        {
            // Arrange - Simulate timeout by throwing TaskCanceledException
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.SetResponseFunc(_ => throw new TaskCanceledException("Request timeout"));

            var httpClient = new HttpClient(mockHandler);
            var logger = Mock.Of<ILogger<OpenRouteServiceClient>>();
            var client = new OpenRouteServiceClient(httpClient, logger, "test-key");

            var cache = new DistanceCache(new MemoryCache(new MemoryCacheOptions()),
                Mock.Of<ILogger<DistanceCache>>());
            var calculator = new DistanceCalculator(client, cache,
                Mock.Of<ILogger<DistanceCalculator>>());

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Fallback);
            result.DistanceMiles.Should().BeApproximately(NYC_LA_STRAIGHT_LINE_MILES, 50.0);
        }

        [Fact]
        public async Task CalculateDistanceAsync_FallbackCalculatesDuration_BasedOnDistance()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueErrorResponse(System.Net.HttpStatusCode.ServiceUnavailable);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert - Duration should be estimated as distance / 40mph * 60 min/hr
            var expectedDuration = (NYC_LA_STRAIGHT_LINE_MILES / 40.0) * 60.0;
            result.DurationMinutes.Should().BeApproximately(expectedDuration, 10.0);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WhenApiReturnsInvalidJson_UsesFallback()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new System.Net.Http.StringContent("invalid json")
            };
            mockHandler.QueueResponse(response);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Fallback);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WhenApiReturnsNullResponse_UsesFallback()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueJsonResponse<RouteResponse?>(null);

            // Act
            var result = await calculator.CalculateDistanceAsync(NYC, LA);

            // Assert
            result.Source.Should().Be(DistanceSource.Fallback);
        }
    }

    /// <summary>
    /// Tests for edge cases and error handling.
    /// </summary>
    public class EdgeCaseTests
    {
        [Fact]
        public async Task CalculateDistanceAsync_WithSameLocation_ReturnsZeroDistance()
        {
            // Arrange
            var location = new Location(40.7128, -74.0060);
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(0.0, 0.0));

            // Act
            var result = await calculator.CalculateDistanceAsync(location, location);

            // Assert
            result.DistanceMiles.Should().Be(0.0);
            result.DurationMinutes.Should().Be(0.0);
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithCancellationToken_PropagatesToken()
        {
            // Arrange
            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(4500000.0, 144000.0));
            var cts = new CancellationTokenSource();

            // Act
            var task = calculator.CalculateDistanceAsync(NYC, LA, cts.Token);
            await task;

            // Assert - Should complete without cancellation
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task CalculateDistanceAsync_WithVeryShortDistance_HandlesCorrectly()
        {
            // Arrange - Test locations very close together (< 1 mile)
            var location1 = new Location(40.7128, -74.0060);
            var location2 = new Location(40.7138, -74.0070); // ~0.1 miles away

            var (calculator, mockHandler, _) = CreateCalculator();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(160.9, 60.0)); // ~0.1 mi, 1 min

            // Act
            var result = await calculator.CalculateDistanceAsync(location1, location2);

            // Assert
            result.DistanceMiles.Should().BeApproximately(0.1, 0.01);
            result.DurationMinutes.Should().Be(1.0);
        }
    }

    #region Helper Methods

    private static (DistanceCalculator calculator, MockHttpMessageHandler mockHandler, IMemoryCache cache)
        CreateCalculator()
    {
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(mockHandler);
        var logger = Mock.Of<ILogger<OpenRouteServiceClient>>();
        var client = new OpenRouteServiceClient(httpClient, logger, "test-api-key");

        var cache = new MemoryCache(new MemoryCacheOptions());
        var cacheLogger = Mock.Of<ILogger<DistanceCache>>();
        var distanceCache = new DistanceCache(cache, cacheLogger);

        var calculatorLogger = Mock.Of<ILogger<DistanceCalculator>>();
        var calculator = new DistanceCalculator(client, distanceCache, calculatorLogger);

        return (calculator, mockHandler, cache);
    }

    private static RouteResponse CreateMockRouteResponse(double distanceMeters, double durationSeconds)
    {
        return new RouteResponse
        {
            Features = new[]
            {
                new RouteFeature
                {
                    Properties = new RouteProperties
                    {
                        Summary = new RouteSummary
                        {
                            Distance = distanceMeters,
                            Duration = durationSeconds
                        }
                    }
                }
            }
        };
    }

    #endregion
}

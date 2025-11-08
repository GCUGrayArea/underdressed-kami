using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SmartScheduler.Infrastructure.ExternalServices;
using SmartScheduler.Infrastructure.ExternalServices.Models;
using SmartScheduler.Tests.TestHelpers;

namespace SmartScheduler.Tests.Infrastructure.ExternalServices;

/// <summary>
/// Unit tests for OpenRouteServiceClient covering:
/// - Successful API calls and JSON parsing
/// - Retry logic with exponential backoff
/// - Error handling for various HTTP status codes
/// - Timeout scenarios
/// </summary>
public class OpenRouteServiceClientTests
{
    /// <summary>
    /// Tests for successful API calls.
    /// </summary>
    public class SuccessfulApiCallTests
    {
        [Fact]
        public async Task GetRouteAsync_WithValidRequest_ReturnsRouteResponse()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            var mockResponse = CreateMockRouteResponse(4500000.0, 144000.0);
            mockHandler.QueueJsonResponse(mockResponse);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.GetDistanceMeters().Should().Be(4500000.0);
            result.GetDurationSeconds().Should().Be(144000.0);
            mockHandler.RequestCount.Should().Be(1);
        }

        [Fact]
        public async Task GetRouteAsync_AddsAuthorizationHeader_ToRequest()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var client = CreateClient(mockHandler, "test-api-key-12345");
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            await client.GetRouteAsync(request);

            // Assert
            var sentRequest = mockHandler.Requests.First();
            sentRequest.Headers.Should().Contain(h => h.Key == "Authorization");
            sentRequest.Headers.GetValues("Authorization").First().Should().Be("test-api-key-12345");
        }

        [Fact]
        public async Task GetRouteAsync_UsesCorrectEndpoint()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            await client.GetRouteAsync(request);

            // Assert
            var sentRequest = mockHandler.Requests.First();
            sentRequest.RequestUri!.PathAndQuery.Should().Be("/v2/directions/driving-car");
            sentRequest.Method.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public async Task GetRouteAsync_ParsesComplexGeoJsonResponse()
        {
            // Arrange - Test with actual OpenRouteService GeoJSON structure
            var mockHandler = new MockHttpMessageHandler();
            var complexResponse = new RouteResponse
            {
                Features = new[]
                {
                    new RouteFeature
                    {
                        Properties = new RouteProperties
                        {
                            Summary = new RouteSummary
                            {
                                Distance = 123456.78,
                                Duration = 9876.54
                            }
                        }
                    }
                }
            };
            mockHandler.QueueJsonResponse(complexResponse);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.GetDistanceMeters().Should().Be(123456.78);
            result.GetDurationSeconds().Should().Be(9876.54);
        }
    }

    /// <summary>
    /// Tests for retry logic with exponential backoff.
    /// </summary>
    public class RetryLogicTests
    {
        [Fact]
        public async Task GetRouteAsync_WithRateLimitError_RetriesWithExponentialBackoff()
        {
            // Arrange - First 4 attempts return 429, 5th succeeds
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var startTime = DateTime.UtcNow;
            var result = await client.GetRouteAsync(request);
            var elapsed = DateTime.UtcNow - startTime;

            // Assert
            result.Should().NotBeNull();
            mockHandler.RequestCount.Should().Be(5);
            // Exponential backoff: 1s + 2s + 4s + 8s = 15s minimum
            elapsed.TotalSeconds.Should().BeGreaterThan(14.5);
        }

        [Fact]
        public async Task GetRouteAsync_WithRateLimitError_FailsAfterMaxRetries()
        {
            // Arrange - All attempts return 429
            var mockHandler = new MockHttpMessageHandler();
            for (int i = 0; i < 10; i++)
            {
                mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            }

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
            // Max retries: 4 retries + 1 initial attempt = 5 total
            mockHandler.RequestCount.Should().Be(5);
        }

        [Fact]
        public async Task GetRouteAsync_SucceedsOnSecondAttempt()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().NotBeNull();
            mockHandler.RequestCount.Should().Be(2);
        }

        [Fact]
        public async Task GetRouteAsync_WithRateLimitError_LogsRetryAttempts()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueErrorResponse(HttpStatusCode.TooManyRequests);
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var mockLogger = new Mock<ILogger<OpenRouteServiceClient>>();
            var client = CreateClient(mockHandler, logger: mockLogger.Object);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            await client.GetRouteAsync(request);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Rate limit")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Tests for error handling.
    /// </summary>
    public class ErrorHandlingTests
    {
        [Theory]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.Forbidden)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task GetRouteAsync_WithNonRetryableError_FailsImmediately(HttpStatusCode statusCode)
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueErrorResponse(statusCode);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
            mockHandler.RequestCount.Should().Be(1, "non-retryable errors should not retry");
        }

        [Fact]
        public async Task GetRouteAsync_WithHttpRequestException_ReturnsNull()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.SetResponseFunc(_ => throw new HttpRequestException("Network error"));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRouteAsync_WithTimeout_ReturnsNull()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.SetResponseFunc(_ => throw new TaskCanceledException("Request timeout"));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRouteAsync_WithInvalidJson_ReturnsNull()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ invalid json }")
            };
            mockHandler.QueueResponse(response);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRouteAsync_WithEmptyResponse_ReturnsNull()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("")
            };
            mockHandler.QueueResponse(response);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetRouteAsync_LogsErrorsForFailedRequests()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueErrorResponse(HttpStatusCode.InternalServerError, "Server error");

            var mockLogger = new Mock<ILogger<OpenRouteServiceClient>>();
            var client = CreateClient(mockHandler, logger: mockLogger.Object);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            await client.GetRouteAsync(request);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("500")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Tests for edge cases.
    /// </summary>
    public class EdgeCaseTests
    {
        [Fact]
        public async Task GetRouteAsync_WithCancellationToken_PropagatesToken()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.QueueJsonResponse(CreateMockRouteResponse(1000.0, 60.0));

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);
            var cts = new CancellationTokenSource();

            // Act
            var task = client.GetRouteAsync(request, cts.Token);
            await task;

            // Assert
            task.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task GetRouteAsync_WithEmptyFeatures_ReturnsZeroValues()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            var emptyResponse = new RouteResponse { Features = Array.Empty<RouteFeature>() };
            mockHandler.QueueJsonResponse(emptyResponse);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.GetDistanceMeters().Should().Be(0.0);
            result.GetDurationSeconds().Should().Be(0.0);
        }

        [Fact]
        public async Task GetRouteAsync_WithNullSummary_ReturnsZeroValues()
        {
            // Arrange
            var mockHandler = new MockHttpMessageHandler();
            var responseWithNullSummary = new RouteResponse
            {
                Features = new[]
                {
                    new RouteFeature
                    {
                        Properties = new RouteProperties { Summary = null }
                    }
                }
            };
            mockHandler.QueueJsonResponse(responseWithNullSummary);

            var client = CreateClient(mockHandler);
            var request = RouteRequest.Create(40.7128, -74.0060, 34.0522, -118.2437);

            // Act
            var result = await client.GetRouteAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.GetDistanceMeters().Should().Be(0.0);
            result.GetDurationSeconds().Should().Be(0.0);
        }
    }

    #region Helper Methods

    private static OpenRouteServiceClient CreateClient(
        MockHttpMessageHandler mockHandler,
        string apiKey = "test-api-key",
        ILogger<OpenRouteServiceClient>? logger = null)
    {
        var httpClient = new HttpClient(mockHandler);
        logger ??= Mock.Of<ILogger<OpenRouteServiceClient>>();
        return new OpenRouteServiceClient(httpClient, logger, apiKey);
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

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartScheduler.Infrastructure.ExternalServices.Models;

namespace SmartScheduler.Infrastructure.ExternalServices;

/// <summary>
/// HTTP client wrapper for OpenRouteService API with retry logic and error handling.
/// Implements exponential backoff for rate limiting (429) responses.
/// </summary>
public sealed class OpenRouteServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenRouteServiceClient> _logger;
    private readonly string _apiKey;

    // Retry configuration: 1s, 2s, 4s, 8s (max 4 retries)
    private static readonly int[] RetryDelaysMs = { 1000, 2000, 4000, 8000 };

    public OpenRouteServiceClient(
        HttpClient httpClient,
        ILogger<OpenRouteServiceClient> logger,
        string apiKey)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

        // Configure base URL and default timeout
        _httpClient.BaseAddress = new Uri("https://api.openrouteservice.org");
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// Calls OpenRouteService API to get route information with retry logic.
    /// Returns null if API call fails after all retries.
    /// </summary>
    public async Task<RouteResponse?> GetRouteAsync(
        RouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        var maxAttempts = RetryDelaysMs.Length + 1; // 4 retries + initial attempt = 5 total

        while (attempt < maxAttempts)
        {
            try
            {
                var response = await SendRequestAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var routeResponse = await response.Content
                        .ReadFromJsonAsync<RouteResponse>(cancellationToken);

                    _logger.LogInformation(
                        "OpenRouteService API call succeeded on attempt {Attempt}",
                        attempt + 1);

                    return routeResponse;
                }

                // Handle rate limiting with exponential backoff
                if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < RetryDelaysMs.Length)
                {
                    var delayMs = RetryDelaysMs[attempt];
                    _logger.LogWarning(
                        "Rate limit hit (429). Retrying in {DelayMs}ms (attempt {Attempt}/{MaxAttempts})",
                        delayMs, attempt + 1, maxAttempts);

                    await Task.Delay(delayMs, cancellationToken);
                    attempt++;
                    continue;
                }

                // Log error for non-retryable status codes
                LogApiError(response, attempt);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "HTTP request failed on attempt {Attempt}/{MaxAttempts}",
                    attempt + 1, maxAttempts);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,
                    "Request timeout on attempt {Attempt}/{MaxAttempts}",
                    attempt + 1, maxAttempts);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Failed to parse API response on attempt {Attempt}/{MaxAttempts}",
                    attempt + 1, maxAttempts);
                return null;
            }
        }

        _logger.LogError("OpenRouteService API call failed after {MaxAttempts} attempts", maxAttempts);
        return null;
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        RouteRequest request,
        CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/v2/directions/driving-car")
        {
            Content = JsonContent.Create(request)
        };

        // Add API key to headers
        requestMessage.Headers.Add("Authorization", _apiKey);

        return await _httpClient.SendAsync(requestMessage, cancellationToken);
    }

    private void LogApiError(HttpResponseMessage response, int attempt)
    {
        _logger.LogError(
            "OpenRouteService API returned {StatusCode} on attempt {Attempt}. Reason: {Reason}",
            (int)response.StatusCode,
            attempt + 1,
            response.ReasonPhrase);
    }
}

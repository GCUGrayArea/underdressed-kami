using System.Net;
using System.Text;
using System.Text.Json;

namespace SmartScheduler.Tests.TestHelpers;

/// <summary>
/// Custom HttpMessageHandler for mocking HTTP responses in tests.
/// Allows configuring different responses for different requests.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses = new();
    private readonly List<HttpRequestMessage> _requests = new();
    private Func<HttpRequestMessage, HttpResponseMessage>? _responseFunc;

    /// <summary>
    /// Gets all requests that were sent through this handler.
    /// </summary>
    public IReadOnlyList<HttpRequestMessage> Requests => _requests;

    /// <summary>
    /// Gets the number of requests that were sent.
    /// </summary>
    public int RequestCount => _requests.Count;

    /// <summary>
    /// Queues a response to be returned on the next request.
    /// </summary>
    public void QueueResponse(HttpResponseMessage response)
    {
        _responses.Enqueue(response);
    }

    /// <summary>
    /// Queues a successful JSON response.
    /// </summary>
    public void QueueJsonResponse<T>(T content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var json = JsonSerializer.Serialize(content);
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        QueueResponse(response);
    }

    /// <summary>
    /// Queues an error response.
    /// </summary>
    public void QueueErrorResponse(HttpStatusCode statusCode, string? reason = null)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            ReasonPhrase = reason
        };
        QueueResponse(response);
    }

    /// <summary>
    /// Sets a function that will be called for each request to generate a response.
    /// This overrides any queued responses.
    /// </summary>
    public void SetResponseFunc(Func<HttpRequestMessage, HttpResponseMessage> func)
    {
        _responseFunc = func;
    }

    /// <summary>
    /// Clears all queued responses and requests.
    /// </summary>
    public void Reset()
    {
        _responses.Clear();
        _requests.Clear();
        _responseFunc = null;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _requests.Add(request);

        // Use custom response function if set
        if (_responseFunc != null)
        {
            return Task.FromResult(_responseFunc(request));
        }

        // Use queued responses
        if (_responses.Count > 0)
        {
            return Task.FromResult(_responses.Dequeue());
        }

        // Default to 404 if no response configured
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}

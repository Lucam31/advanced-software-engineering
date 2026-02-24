using System.Net;
using System.Text;
using Shared.Logger;

namespace UnitTesting.Mock;

/// <summary>
/// Represents a single recorded HTTP request made through the mock handler.
/// </summary>
public class RecordedRequest
{
    /// <summary>
    /// The HTTP method of the request (GET, POST, etc.).
    /// </summary>
    public HttpMethod Method { get; init; } = HttpMethod.Get;

    /// <summary>
    /// The full request URI.
    /// </summary>
    public Uri? RequestUri { get; init; }

    /// <summary>
    /// The request body as a string, or null if no body was sent.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// The Content-Type header value of the request.
    /// </summary>
    public string? ContentType { get; init; }
}

/// <summary>
/// A mock <see cref="HttpMessageHandler"/> that intercepts HTTP requests,
/// records them for later inspection, and returns configurable responses.
/// This allows testing of API client code without a real server.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses = new();
    private readonly List<RecordedRequest> _recordedRequests = new();

    /// <summary>
    /// Gets all recorded requests that were sent through this handler.
    /// </summary>
    public IReadOnlyList<RecordedRequest> RecordedRequests => _recordedRequests.AsReadOnly();

    /// <summary>
    /// Enqueues a response that will be returned for the next HTTP request.
    /// Responses are returned in FIFO order.
    /// </summary>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    /// <param name="content">The response body as a string. Defaults to empty.</param>
    /// <param name="contentType">The Content-Type of the response. Defaults to "application/json".</param>
    public void EnqueueResponse(HttpStatusCode statusCode, string content = "", string contentType = "application/json")
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, contentType)
        };
        _responses.Enqueue(response);
    }

    /// <summary>
    /// Intercepts the HTTP request, records it, and returns the next enqueued response.
    /// </summary>
    /// <param name="request">The outgoing HTTP request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The next enqueued <see cref="HttpResponseMessage"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no response has been enqueued.</exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content != null
            ? await request.Content.ReadAsStringAsync(cancellationToken)
            : null;

        _recordedRequests.Add(new RecordedRequest
        {
            Method = request.Method,
            RequestUri = request.RequestUri,
            Body = body,
            ContentType = request.Content?.Headers.ContentType?.MediaType
        });

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException(
                $"No response enqueued for {request.Method} {request.RequestUri}. " +
                "Use EnqueueResponse() to configure expected responses before making requests.");
        }

        return _responses.Dequeue();
    }

    /// <summary>
    /// Returns the recorded request at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the request.</param>
    /// <returns>The recorded request.</returns>
    public RecordedRequest GetRequest(int index) => _recordedRequests[index];

    /// <summary>
    /// Clears all recorded requests and enqueued responses.
    /// </summary>
    public void Clear()
    {
        _recordedRequests.Clear();
        _responses.Clear();
    }
}


using System.Net;
using System.Text;
using System.Text.Json;

namespace stockytests.Mocks;

/// <summary>
/// A custom HttpMessageHandler that returns mock responses based on MockCallFile data.
/// </summary>
public sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly MockCallFile _mockCall;
    private readonly bool _strictUrlMatching;

    /// <summary>
    /// Creates a new mock handler that will return the response from the provided mock call.
    /// </summary>
    /// <param name="mockCall">The mock call containing request/response data</param>
    /// <param name="strictUrlMatching">If true, validates that request URL matches mock URL</param>
    public MockHttpMessageHandler(MockCallFile mockCall, bool strictUrlMatching = false)
    {
        _mockCall = mockCall ?? throw new ArgumentNullException(nameof(mockCall));
        _strictUrlMatching = strictUrlMatching;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_strictUrlMatching)
        {
            ValidateRequestUrl(request);
        }

        var response = CreateResponse();
        return Task.FromResult(response);
    }

    private void ValidateRequestUrl(HttpRequestMessage request)
    {
        var requestUrl = NormalizeUrl(request.RequestUri?.ToString() ?? string.Empty);
        var mockUrl = NormalizeUrl(_mockCall.Request.Url);

        if (!UrlsMatch(requestUrl, mockUrl))
        {
            throw new InvalidOperationException(
                $"Request URL does not match mock URL.\n" +
                $"Request: {request.RequestUri}\n" +
                $"Mock: {_mockCall.Request.Url}");
        }
    }

    private HttpResponseMessage CreateResponse()
    {
        var statusCode = (HttpStatusCode)_mockCall.Response.Status;
        var response = new HttpResponseMessage(statusCode)
        {
            ReasonPhrase = _mockCall.Response.StatusText
        };

        if (_mockCall.Response.BodyJson != null)
        {
            var jsonContent = SerializeBodyJson(_mockCall.Response.BodyJson);
            response.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        foreach (var header in _mockCall.Response.Headers)
        {
            if (!IsContentHeader(header.Key))
            {
                response.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return response;
    }

    private static string SerializeBodyJson(object bodyJson)
    {
        if (bodyJson is JsonElement element)
        {
            return element.GetRawText();
        }

        return JsonSerializer.Serialize(bodyJson);
    }

    private static string NormalizeUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        var uri = new Uri(url, UriKind.Absolute);
        return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
    }

    private static bool UrlsMatch(string url1, string url2)
    {
        return string.Equals(url1, url2, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsContentHeader(string headerName)
    {
        return headerName.Equals("content-type", StringComparison.OrdinalIgnoreCase) ||
               headerName.Equals("content-length", StringComparison.OrdinalIgnoreCase) ||
               headerName.Equals("content-encoding", StringComparison.OrdinalIgnoreCase);
    }
}

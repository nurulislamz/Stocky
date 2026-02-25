using System.Text.Json.Serialization;

namespace stockytests.Mocks;

/// <summary>
/// Represents a mock API call file containing request and response data.
/// </summary>
public sealed class MockCallFile
{
    [JsonPropertyName("request")]
    public MockCallRequest Request { get; set; } = null!;

    [JsonPropertyName("response")]
    public MockCallResponse Response { get; set; } = null!;
}

/// <summary>
/// Represents the request portion of a mock call.
/// </summary>
public sealed class MockCallRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("method")]
    public string Method { get; set; } = "GET";

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = new();
}

/// <summary>
/// Represents the response portion of a mock call.
/// </summary>
public sealed class MockCallResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("statusText")]
    public string StatusText { get; set; } = string.Empty;

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = new();

    [JsonPropertyName("bodyJson")]
    public object? BodyJson { get; set; }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// Shared base
// ------------------------------------------------------------
public abstract class YahooFinanceDto
{
    // Capture any fields yahoo-finance2 returns that we haven't explicitly modeled.
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}

// Optional helper if you want to keep "raw/fmt" style values later.
// yahoo-finance2 often already normalizes values, but quoteSummary may include these.
public sealed class YahooValue<T> : YahooFinanceDto
{
    [JsonPropertyName("raw")]
    public T? Raw { get; set; }

    [JsonPropertyName("fmt")]
    public string? Fmt { get; set; }
}

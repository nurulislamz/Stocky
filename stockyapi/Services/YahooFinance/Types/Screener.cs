using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// screener  (JSR: ScreenerResult + ScreenerQuote etc.)
// ------------------------------------------------------------

public sealed class ScreenerResult : YahooFinanceDto
{
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("quotes")]
    public List<ScreenerQuote> Quotes { get; set; } = [];
}

/// <summary>
/// JSR has a dedicated ScreenerQuote interface (very large).
/// We model key common fields and keep the rest in Extra.
/// </summary>
public sealed class ScreenerQuote : QuoteBase
{
    // ScreenerQuote extends/overlaps quote fields; Extra captures the many additional screener-specific fields.
}

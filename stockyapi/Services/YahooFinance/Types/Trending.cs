using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// trendingSymbols  (JSR: TrendingSymbolsResult with quotes list of TrendingSymbol)
// ------------------------------------------------------------

public sealed class TrendingSymbolsResult : YahooFinanceDto
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("jobTimestamp")]
    public long JobTimestamp { get; set; }

    [JsonPropertyName("startInterval")]
    public long StartInterval { get; set; }

    [JsonPropertyName("quotes")]
    public List<TrendingSymbol> Quotes { get; set; } = [];
}

public sealed class TrendingSymbol : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;
}

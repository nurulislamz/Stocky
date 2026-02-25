using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// trendingSymbols  (JSR: TrendingSymbolsResult with quotes list of TrendingSymbol)
// Yahoo API returns: { "finance": { "result": [...], "error": null } }
// ------------------------------------------------------------

/// <summary>
/// Root response wrapper for the trending symbols endpoint.
/// </summary>
public sealed class TrendingSymbolsResponse : YahooFinanceDto
{
    [JsonPropertyName("finance")]
    public TrendingSymbolsFinance Finance { get; set; } = null!;
}

/// <summary>
/// Finance wrapper containing the result array.
/// </summary>
public sealed class TrendingSymbolsFinance : YahooFinanceDto
{
    [JsonPropertyName("result")]
    public List<TrendingSymbolsResult> Result { get; set; } = [];

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

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

using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// recommendationsBySymbol  (JSR: RecommendationsBySymbolResponse, plus array alias)
// Yahoo API returns: { "finance": { "result": [...], "error": null } }
// ------------------------------------------------------------

/// <summary>
/// Root response wrapper for the recommendations by symbol endpoint.
/// </summary>
public sealed class RecommendationsBySymbolApiResponse : YahooFinanceDto
{
    [JsonPropertyName("finance")]
    public RecommendationsBySymbolFinance Finance { get; set; } = null!;
}

/// <summary>
/// Finance wrapper containing the result array.
/// </summary>
public sealed class RecommendationsBySymbolFinance : YahooFinanceDto
{
    [JsonPropertyName("result")]
    public List<RecommendationsBySymbolResponse> Result { get; set; } = [];

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

public sealed class RecommendationsBySymbolResponse : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("recommendedSymbols")]
    public List<RecommendedSymbol> RecommendedSymbols { get; set; } = [];
}

public sealed class RecommendationsBySymbolResponseArray : List<RecommendationsBySymbolResponse> { }

public sealed class RecommendedSymbol : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("score")]
    public decimal Score { get; set; }
}

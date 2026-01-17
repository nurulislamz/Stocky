using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// recommendationsBySymbol  (JSR: RecommendationsBySymbolResponse, plus array alias)
// ------------------------------------------------------------

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

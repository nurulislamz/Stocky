using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// quote  (JSR: quote() returns array by default; can return object via options)
// ------------------------------------------------------------

/// <summary>
/// JSR type alias equivalent: QuoteResponseArray (default return type) => Quote[]
/// We model the default array return.
/// </summary>
public sealed class QuoteResponseArray : List<Quote> { }

/// <summary>
/// JSR: Quote is a union of quote types (equity/etf/crypto/option/etc).
/// We model the common base (QuoteBase) and keep the rest flexible.
/// </summary>
public sealed class Quote : QuoteBase
{
    // union-specific fields are captured by Extra
}

public class QuoteBase : YahooFinanceDto
{
    // The repo type list is huge; these are the core common fields used everywhere.
    // Everything else remains available via Extra.

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("quoteType")]
    public string? QuoteType { get; set; } // e.g. "EQUITY", "ETF", "CRYPTOCURRENCY", "OPTION", etc.

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("longName")]
    public string? LongName { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("fullExchangeName")]
    public string? FullExchangeName { get; set; }

    [JsonPropertyName("marketState")]
    public string? MarketState { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public decimal? RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public decimal? RegularMarketChangePercent { get; set; }

    [JsonPropertyName("regularMarketTime")]
    public long? RegularMarketTime { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long? RegularMarketVolume { get; set; }

    [JsonPropertyName("marketCap")]
    public long? MarketCap { get; set; }

    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("bidSize")]
    public long? BidSize { get; set; }

    [JsonPropertyName("askSize")]
    public long? AskSize { get; set; }
}

public sealed class QuoteAltSymbol : YahooFinanceDto
{
    [JsonPropertyName("expireDate")]
    public long? ExpireDate { get; set; }

    [JsonPropertyName("expireIsoDate")]
    public string? ExpireIsoDate { get; set; }

    [JsonPropertyName("quoteType")]
    public string? QuoteType { get; set; }

    [JsonPropertyName("typeDisp")]
    public string? TypeDisp { get; set; }

    [JsonPropertyName("underlyingExchangeSymbol")]
    public string? UnderlyingExchangeSymbol { get; set; }
}

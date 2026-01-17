using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// options  (JSR: OptionsResult -> expirationDates/strikes/quote/options/underlyingSymbol)
// ------------------------------------------------------------

public sealed class OptionsResult : YahooFinanceDto
{
    [JsonPropertyName("expirationDates")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetListConverter))]
    public List<DateTimeOffset> ExpirationDates { get; set; } = [];

    [JsonPropertyName("hasMiniOptions")]
    public bool HasMiniOptions { get; set; }

    [JsonPropertyName("options")]
    public List<Option> Options { get; set; } = [];

    [JsonPropertyName("quote")]
    public Quote Quote { get; set; } = null!;

    [JsonPropertyName("strikes")]
    public List<decimal> Strikes { get; set; } = [];

    [JsonPropertyName("underlyingSymbol")]
    public string UnderlyingSymbol { get; set; } = null!;
}

public sealed class Option : YahooFinanceDto
{
    [JsonPropertyName("expirationDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset ExpirationDate { get; set; }

    [JsonPropertyName("hasMiniOptions")]
    public bool HasMiniOptions { get; set; }

    [JsonPropertyName("calls")]
    public List<CallOrPut> Calls { get; set; } = [];

    [JsonPropertyName("puts")]
    public List<CallOrPut> Puts { get; set; } = [];
}

public sealed class CallOrPut : YahooFinanceDto
{
    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName("change")]
    public decimal? Change { get; set; }

    [JsonPropertyName("contractSize")]
    public string ContractSize { get; set; } = null!;

    [JsonPropertyName("contractSymbol")]
    public string ContractSymbol { get; set; } = null!;

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("expiration")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Expiration { get; set; }

    [JsonPropertyName("impliedVolatility")]
    public decimal ImpliedVolatility { get; set; }

    [JsonPropertyName("inTheMoney")]
    public bool InTheMoney { get; set; }

    [JsonPropertyName("lastPrice")]
    public decimal LastPrice { get; set; }

    [JsonPropertyName("lastTradeDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset LastTradeDate { get; set; }

    [JsonPropertyName("openInterest")]
    public long? OpenInterest { get; set; }

    [JsonPropertyName("percentChange")]
    public decimal? PercentChange { get; set; }

    [JsonPropertyName("strike")]
    public decimal Strike { get; set; }

    [JsonPropertyName("volume")]
    public long? Volume { get; set; }

    // Greeks may appear; keep flexible via Extra.
}

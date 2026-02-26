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

    [JsonPropertyName("language")]
    public string Language { get; set; } = null!;

    [JsonPropertyName("region")]
    public string Region { get; set; } = null!;

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = null!; // e.g. "EQUITY", "ETF", "CRYPTOCURRENCY", "OPTION", etc.

    [JsonPropertyName("typeDisp")]
    public string? TypeDisp { get; set; }

    [JsonPropertyName("quoteSourceName")]
    public string? QuoteSourceName { get; set; }

    [JsonPropertyName("triggerable")]
    public bool Triggerable { get; set; }

    [JsonPropertyName("customPriceAlertConfidence")]
    public string? CustomPriceAlertConfidence { get; set; }

    [JsonPropertyName("marketState")]
    public string MarketState { get; set; } = null!;

    [JsonPropertyName("tradeable")]
    public bool Tradeable { get; set; }

    [JsonPropertyName("cryptoTradeable")]
    public bool? CryptoTradeable { get; set; }

    [JsonPropertyName("corporateActions")]
    public List<object>? CorporateActions { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("longName")]
    public string? LongName { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = null!;

    [JsonPropertyName("fullExchangeName")]
    public string FullExchangeName { get; set; } = null!;

    [JsonPropertyName("exchangeTimezoneName")]
    public string ExchangeTimezoneName { get; set; } = null!;

    [JsonPropertyName("exchangeTimezoneShortName")]
    public string ExchangeTimezoneShortName { get; set; } = null!;

    [JsonPropertyName("gmtOffSetMilliseconds")]
    public int GmtOffSetMilliseconds { get; set; }

    [JsonPropertyName("market")]
    public string Market { get; set; } = null!;

    [JsonPropertyName("esgPopulated")]
    public bool EsgPopulated { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public decimal? RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public decimal? RegularMarketChangePercent { get; set; }

    [JsonPropertyName("regularMarketTime")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? RegularMarketTime { get; set; }

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

    [JsonPropertyName("sourceInterval")]
    public int SourceInterval { get; set; }

    [JsonPropertyName("exchangeDataDelayedBy")]
    public int ExchangeDataDelayedBy { get; set; }

    [JsonPropertyName("firstTradeDateMilliseconds")]
    [JsonConverter(typeof(UnixMillisecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? FirstTradeDateMilliseconds { get; set; }

    [JsonPropertyName("priceHint")]
    public int? PriceHint { get; set; }

    [JsonPropertyName("postMarketChangePercent")]
    public decimal? PostMarketChangePercent { get; set; }

    [JsonPropertyName("postMarketTime")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? PostMarketTime { get; set; }

    [JsonPropertyName("postMarketPrice")]
    public decimal? PostMarketPrice { get; set; }

    [JsonPropertyName("postMarketChange")]
    public decimal? PostMarketChange { get; set; }

    [JsonPropertyName("hasPrePostMarketData")]
    public bool? HasPrePostMarketData { get; set; }

    [JsonPropertyName("regularMarketDayHigh")]
    public decimal? RegularMarketDayHigh { get; set; }

    [JsonPropertyName("regularMarketDayRange")]
    public string? RegularMarketDayRange { get; set; }

    [JsonPropertyName("regularMarketDayLow")]
    public decimal? RegularMarketDayLow { get; set; }

    [JsonPropertyName("regularMarketPreviousClose")]
    public decimal? RegularMarketPreviousClose { get; set; }

    [JsonPropertyName("regularMarketOpen")]
    public decimal? RegularMarketOpen { get; set; }

    [JsonPropertyName("averageDailyVolume3Month")]
    public long? AverageDailyVolume3Month { get; set; }

    [JsonPropertyName("averageDailyVolume10Day")]
    public long? AverageDailyVolume10Day { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("underlyingSymbol")]
    public string? UnderlyingSymbol { get; set; }

    [JsonPropertyName("ytdReturn")]
    public decimal? YtdReturn { get; set; }

    [JsonPropertyName("trailingThreeMonthReturns")]
    public decimal? TrailingThreeMonthReturns { get; set; }

    [JsonPropertyName("trailingThreeMonthNavReturns")]
    public decimal? TrailingThreeMonthNavReturns { get; set; }

    [JsonPropertyName("ipoExpectedDate")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? IpoExpectedDate { get; set; }

    [JsonPropertyName("newListingDate")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? NewListingDate { get; set; }

    [JsonPropertyName("nameChangeDate")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? NameChangeDate { get; set; }

    [JsonPropertyName("prevName")]
    public string? PrevName { get; set; }

    [JsonPropertyName("averageAnalystRating")]
    public string? AverageAnalystRating { get; set; }

    [JsonPropertyName("pageViewGrowthWeekly")]
    public decimal? PageViewGrowthWeekly { get; set; }

    [JsonPropertyName("openInterest")]
    public long? OpenInterest { get; set; }

    [JsonPropertyName("beta")]
    public decimal? Beta { get; set; }

    [JsonPropertyName("companyLogoUrl")]
    public string? CompanyLogoUrl { get; set; }

    [JsonPropertyName("logoUrl")]
    public string? LogoUrl { get; set; }
}

public sealed class QuoteAltSymbol : YahooFinanceDto
{
    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = null!;

    [JsonPropertyName("typeDisp")]
    public string? TypeDisp { get; set; }

    [JsonPropertyName("underlyingExchangeSymbol")]
    public string? UnderlyingExchangeSymbol { get; set; }

    [JsonPropertyName("expireDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset ExpireDate { get; set; }

    [JsonPropertyName("expireIsoDate")]
    public string ExpireIsoDate { get; set; } = null!;
}

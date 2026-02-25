using System.Text.Json;
using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// screener  (JSR: ScreenerResult + ScreenerQuote etc.)
// Yahoo API returns: { "finance": { "result": [...], "error": null } }
// ------------------------------------------------------------

/// <summary>
/// Root response wrapper for the screener endpoint.
/// </summary>
public sealed class ScreenerResponse : YahooFinanceDto
{
    [JsonPropertyName("finance")]
    public ScreenerFinance Finance { get; set; } = null!;
}

/// <summary>
/// Finance wrapper containing the result array.
/// </summary>
public sealed class ScreenerFinance : YahooFinanceDto
{
    [JsonPropertyName("result")]
    public List<ScreenerResult> Result { get; set; } = [];

    [JsonPropertyName("error")]
    public object? Error { get; set; }
}

public sealed class ScreenerResult : YahooFinanceDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("canonicalName")]
    public string CanonicalName { get; set; } = string.Empty;

    [JsonPropertyName("criteriaMeta")]
    public ScreenerCriteriaMeta CriteriaMeta { get; set; } = new();

    [JsonPropertyName("rawCriteria")]
    public string RawCriteria { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public int Start { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("quotes")]
    public List<ScreenerQuote> Quotes { get; set; } = [];

    [JsonPropertyName("useRecords")]
    public bool UseRecords { get; set; }

    [JsonPropertyName("predefinedScr")]
    public bool PredefinedScr { get; set; }

    [JsonPropertyName("versionId")]
    public int VersionId { get; set; }

    [JsonPropertyName("creationDate")]
    public long CreationDate { get; set; }

    [JsonPropertyName("lastUpdated")]
    public long LastUpdated { get; set; }

    [JsonPropertyName("isPremium")]
    public bool IsPremium { get; set; }

    [JsonPropertyName("iconUrl")]
    public string IconUrl { get; set; } = string.Empty;
}

public sealed class ScreenerCriteriaMeta : YahooFinanceDto
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("sortField")]
    public string SortField { get; set; } = string.Empty;

    [JsonPropertyName("sortType")]
    public string SortType { get; set; } = string.Empty;

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = string.Empty;

    [JsonPropertyName("criteria")]
    public List<ScreenerCriterum> Criteria { get; set; } = [];

    [JsonPropertyName("topOperator")]
    public string TopOperator { get; set; } = string.Empty;

    [JsonPropertyName("includeFields")]
    public List<string> IncludeFields { get; set; } = [];
}

public sealed class ScreenerCriterum : YahooFinanceDto
{
    [JsonPropertyName("field")]
    public string Field { get; set; } = null!;

    [JsonPropertyName("operators")]
    public List<string> Operators { get; set; } = [];

    [JsonPropertyName("values")]
    public List<JsonElement> Values { get; set; } = [];

    [JsonPropertyName("labelsSelected")]
    public List<int> LabelsSelected { get; set; } = [];

    [JsonPropertyName("dependentValues")]
    public List<JsonElement> DependentValues { get; set; } = [];

    [JsonPropertyName("subField")]
    public string? SubField { get; set; }
}

public sealed class ScreenerQuote : YahooFinanceDto
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = null!;

    [JsonPropertyName("region")]
    public string Region { get; set; } = null!;

    [JsonPropertyName("quoteType")]
    public string QuoteType { get; set; } = null!;

    [JsonPropertyName("typeDisp")]
    public string TypeDisp { get; set; } = null!;

    [JsonPropertyName("quoteSourceName")]
    public string? QuoteSourceName { get; set; }

    [JsonPropertyName("triggerable")]
    public bool Triggerable { get; set; }

    [JsonPropertyName("customPriceAlertConfidence")]
    public string CustomPriceAlertConfidence { get; set; } = null!;

    [JsonPropertyName("lastCloseTevEbitLtm")]
    public decimal? LastCloseTevEbitLtm { get; set; }

    [JsonPropertyName("lastClosePriceToNNWCPerShare")]
    public decimal? LastClosePriceToNNWCPerShare { get; set; }

    [JsonPropertyName("firstTradeDateMilliseconds")]
    public long? FirstTradeDateMilliseconds { get; set; }

    [JsonPropertyName("priceHint")]
    public int? PriceHint { get; set; }

    [JsonPropertyName("postMarketChangePercent")]
    public decimal? PostMarketChangePercent { get; set; }

    [JsonPropertyName("postMarketTime")]
    public long? PostMarketTime { get; set; }

    [JsonPropertyName("postMarketPrice")]
    public decimal? PostMarketPrice { get; set; }

    [JsonPropertyName("postMarketChange")]
    public decimal? PostMarketChange { get; set; }

    [JsonPropertyName("regularMarketChange")]
    public decimal? RegularMarketChange { get; set; }

    [JsonPropertyName("regularMarketTime")]
    public long? RegularMarketTime { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketDayHigh")]
    public decimal? RegularMarketDayHigh { get; set; }

    [JsonPropertyName("regularMarketDayRange")]
    public string? RegularMarketDayRange { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("regularMarketDayLow")]
    public decimal? RegularMarketDayLow { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long? RegularMarketVolume { get; set; }

    [JsonPropertyName("regularMarketPreviousClose")]
    public decimal? RegularMarketPreviousClose { get; set; }

    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("bidSize")]
    public long? BidSize { get; set; }

    [JsonPropertyName("askSize")]
    public long? AskSize { get; set; }

    [JsonPropertyName("market")]
    public string Market { get; set; } = null!;

    [JsonPropertyName("messageBoardId")]
    public string MessageBoardId { get; set; } = null!;

    [JsonPropertyName("fullExchangeName")]
    public string FullExchangeName { get; set; } = null!;

    [JsonPropertyName("longName")]
    public string? LongName { get; set; }

    [JsonPropertyName("financialCurrency")]
    public string? FinancialCurrency { get; set; }

    [JsonPropertyName("regularMarketOpen")]
    public decimal? RegularMarketOpen { get; set; }

    [JsonPropertyName("averageDailyVolume3Month")]
    public long? AverageDailyVolume3Month { get; set; }

    [JsonPropertyName("averageDailyVolume10Day")]
    public long? AverageDailyVolume10Day { get; set; }

    [JsonPropertyName("fiftyTwoWeekLowChange")]
    public decimal? FiftyTwoWeekLowChange { get; set; }

    [JsonPropertyName("fiftyTwoWeekLowChangePercent")]
    public decimal? FiftyTwoWeekLowChangePercent { get; set; }

    [JsonPropertyName("fiftyTwoWeekRange")]
    public string FiftyTwoWeekRange { get; set; } = null!;

    [JsonPropertyName("fiftyTwoWeekHighChange")]
    public decimal? FiftyTwoWeekHighChange { get; set; }

    [JsonPropertyName("fiftyTwoWeekHighChangePercent")]
    public decimal? FiftyTwoWeekHighChangePercent { get; set; }

    [JsonPropertyName("fiftyTwoWeekChangePercent")]
    public decimal? FiftyTwoWeekChangePercent { get; set; }

    [JsonPropertyName("earningsTimestamp")]
    public long? EarningsTimestamp { get; set; }

    [JsonPropertyName("earningsTimestampStart")]
    public long? EarningsTimestampStart { get; set; }

    [JsonPropertyName("earningsTimestampEnd")]
    public long? EarningsTimestampEnd { get; set; }

    [JsonPropertyName("trailingAnnualDividendRate")]
    public decimal? TrailingAnnualDividendRate { get; set; }

    [JsonPropertyName("trailingAnnualDividendYield")]
    public decimal? TrailingAnnualDividendYield { get; set; }

    [JsonPropertyName("marketState")]
    public string MarketState { get; set; } = null!;

    [JsonPropertyName("epsTrailingTwelveMonths")]
    public decimal? EpsTrailingTwelveMonths { get; set; }

    [JsonPropertyName("epsForward")]
    public decimal? EpsForward { get; set; }

    [JsonPropertyName("epsCurrentYear")]
    public decimal? EpsCurrentYear { get; set; }

    [JsonPropertyName("priceEpsCurrentYear")]
    public decimal? PriceEpsCurrentYear { get; set; }

    [JsonPropertyName("sharesOutstanding")]
    public long? SharesOutstanding { get; set; }

    [JsonPropertyName("bookValue")]
    public decimal? BookValue { get; set; }

    [JsonPropertyName("fiftyDayAverage")]
    public decimal? FiftyDayAverage { get; set; }

    [JsonPropertyName("fiftyDayAverageChange")]
    public decimal? FiftyDayAverageChange { get; set; }

    [JsonPropertyName("fiftyDayAverageChangePercent")]
    public decimal? FiftyDayAverageChangePercent { get; set; }

    [JsonPropertyName("twoHundredDayAverage")]
    public decimal? TwoHundredDayAverage { get; set; }

    [JsonPropertyName("twoHundredDayAverageChange")]
    public decimal? TwoHundredDayAverageChange { get; set; }

    [JsonPropertyName("twoHundredDayAverageChangePercent")]
    public decimal? TwoHundredDayAverageChangePercent { get; set; }

    [JsonPropertyName("marketCap")]
    public long? MarketCap { get; set; }

    [JsonPropertyName("forwardPE")]
    public decimal? ForwardPE { get; set; }

    [JsonPropertyName("priceToBook")]
    public decimal? PriceToBook { get; set; }

    [JsonPropertyName("sourceInterval")]
    public int? SourceInterval { get; set; }

    [JsonPropertyName("exchangeDataDelayedBy")]
    public int? ExchangeDataDelayedBy { get; set; }

    [JsonPropertyName("exchangeTimezoneName")]
    public string ExchangeTimezoneName { get; set; } = null!;

    [JsonPropertyName("exchangeTimezoneShortName")]
    public string ExchangeTimezoneShortName { get; set; } = null!;

    [JsonPropertyName("gmtOffSetMilliseconds")]
    public int? GmtOffSetMilliseconds { get; set; }

    [JsonPropertyName("esgPopulated")]
    public bool EsgPopulated { get; set; }

    [JsonPropertyName("tradeable")]
    public bool Tradeable { get; set; }

    [JsonPropertyName("cryptoTradeable")]
    public bool CryptoTradeable { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = null!;

    [JsonPropertyName("fiftyTwoWeekLow")]
    public decimal? FiftyTwoWeekLow { get; set; }

    [JsonPropertyName("fiftyTwoWeekHigh")]
    public decimal? FiftyTwoWeekHigh { get; set; }

    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = null!;

    [JsonPropertyName("averageAnalystRating")]
    public string? AverageAnalystRating { get; set; }

    [JsonPropertyName("regularMarketChangePercent")]
    public decimal? RegularMarketChangePercent { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("dividendDate")]
    public long? DividendDate { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("trailingPE")]
    public decimal? TrailingPE { get; set; }

    [JsonPropertyName("prevName")]
    public string? PrevName { get; set; }

    [JsonPropertyName("nameChangeDate")]
    public string? NameChangeDate { get; set; }

    [JsonPropertyName("ipoExpectedDate")]
    public string? IpoExpectedDate { get; set; }

    [JsonPropertyName("dividendYield")]
    public decimal? DividendYield { get; set; }

    [JsonPropertyName("dividendRate")]
    public decimal? DividendRate { get; set; }

    [JsonPropertyName("yieldTTM")]
    public decimal? YieldTTM { get; set; }

    [JsonPropertyName("peTTM")]
    public decimal? PeTTM { get; set; }

    [JsonPropertyName("annualReturnNavY3")]
    public decimal? AnnualReturnNavY3 { get; set; }

    [JsonPropertyName("annualReturnNavY5")]
    public decimal? AnnualReturnNavY5 { get; set; }

    [JsonPropertyName("ytdReturn")]
    public decimal? YtdReturn { get; set; }

    [JsonPropertyName("trailingThreeMonthReturns")]
    public decimal? TrailingThreeMonthReturns { get; set; }

    [JsonPropertyName("netAssets")]
    public decimal? NetAssets { get; set; }

    [JsonPropertyName("netExpenseRatio")]
    public decimal? NetExpenseRatio { get; set; }

    [JsonPropertyName("hasPrePostMarketData")]
    public bool? HasPrePostMarketData { get; set; }

    [JsonPropertyName("corporateActions")]
    public List<object>? CorporateActions { get; set; }

    [JsonPropertyName("earningsCallTimestampStart")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? EarningsCallTimestampStart { get; set; }

    [JsonPropertyName("earningsCallTimestampEnd")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? EarningsCallTimestampEnd { get; set; }

    [JsonPropertyName("isEarningsDateEstimate")]
    public bool? IsEarningsDateEstimate { get; set; }

    [JsonPropertyName("preMarketChange")]
    public decimal? PreMarketChange { get; set; }

    [JsonPropertyName("preMarketChangePercent")]
    public decimal? PreMarketChangePercent { get; set; }

    [JsonPropertyName("preMarketTime")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? PreMarketTime { get; set; }

    [JsonPropertyName("preMarketPrice")]
    public decimal? PreMarketPrice { get; set; }
}

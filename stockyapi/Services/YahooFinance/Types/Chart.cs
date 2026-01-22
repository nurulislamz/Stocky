using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// chart  (JSR: ChartResultArray / ChartResultObject + supporting interfaces)
// ------------------------------------------------------------

public sealed class ChartResultArray : YahooFinanceDto
{
    [JsonPropertyName("meta")]
    public ChartMeta? Meta { get; set; }

    [JsonPropertyName("quotes")]
    public List<ChartResultArrayQuote> Quotes { get; set; } = [];

    [JsonPropertyName("events")]
    public ChartEventsArray? Events { get; set; }
}

public sealed class ChartResultArrayQuote : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; set; }

    [JsonPropertyName("open")]
    public decimal? Open { get; set; }

    [JsonPropertyName("high")]
    public decimal? High { get; set; }

    [JsonPropertyName("low")]
    public decimal? Low { get; set; }

    [JsonPropertyName("close")]
    public decimal? Close { get; set; }

    [JsonPropertyName("adjclose")]
    public decimal? Adjclose { get; set; }

    [JsonPropertyName("volume")]
    public long? Volume { get; set; }
}

public sealed class ChartResultObject : YahooFinanceDto
{
    [JsonPropertyName("meta")]
    public ChartMeta? Meta { get; set; }

    [JsonPropertyName("timestamp")]
    public List<long>? Timestamp { get; set; }

    [JsonPropertyName("indicators")]
    public ChartIndicatorsObject? Indicators { get; set; }

    [JsonPropertyName("events")]
    public ChartEventsObject? Events { get; set; }
}

public sealed class ChartMeta : YahooFinanceDto
{
    [JsonPropertyName("chartPreviousClose")]
    public decimal? ChartPreviousClose { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("currentTradingPeriod")]
    public JsonElement? CurrentTradingPeriod { get; set; } // complex (trading periods), keep flexible

    [JsonPropertyName("dataGranularity")]
    public string? DataGranularity { get; set; }

    [JsonPropertyName("exchangeName")]
    public string? ExchangeName { get; set; }

    [JsonPropertyName("exchangeTimezoneName")]
    public string? ExchangeTimezoneName { get; set; }

    [JsonPropertyName("fiftyTwoWeekHigh")]
    public decimal? FiftyTwoWeekHigh { get; set; }

    [JsonPropertyName("fiftyTwoWeekLow")]
    public decimal? FiftyTwoWeekLow { get; set; }

    [JsonPropertyName("firstTradeDate")]
    public long? FirstTradeDate { get; set; }

    [JsonPropertyName("fullExchangeName")]
    public string? FullExchangeName { get; set; }

    [JsonPropertyName("gmtoffset")]
    public int? Gmtoffset { get; set; }

    [JsonPropertyName("hasPrePostMarketData")]
    public bool? HasPrePostMarketData { get; set; }

    [JsonPropertyName("instrumentType")]
    public string? InstrumentType { get; set; }

    [JsonPropertyName("longName")]
    public string? LongName { get; set; }

    [JsonPropertyName("previousClose")]
    public decimal? PreviousClose { get; set; }

    [JsonPropertyName("priceHint")]
    public int? PriceHint { get; set; }

    [JsonPropertyName("range")]
    public string? Range { get; set; }

    [JsonPropertyName("regularMarketDayHigh")]
    public decimal? RegularMarketDayHigh { get; set; }

    [JsonPropertyName("regularMarketDayLow")]
    public decimal? RegularMarketDayLow { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal? RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketTime")]
    public long? RegularMarketTime { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long? RegularMarketVolume { get; set; }

    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("tradingPeriods")]
    public JsonElement? TradingPeriods { get; set; } // complex, keep flexible

    [JsonPropertyName("validRanges")]
    public List<string>? ValidRanges { get; set; }
}

public sealed class ChartIndicatorsObject : YahooFinanceDto
{
    [JsonPropertyName("quote")]
    public List<ChartIndicatorQuote>? Quote { get; set; }

    [JsonPropertyName("adjclose")]
    public List<ChartIndicatorAdjclose>? Adjclose { get; set; }
}

public sealed class ChartIndicatorQuote : YahooFinanceDto
{
    [JsonPropertyName("open")]
    public List<decimal?>? Open { get; set; }

    [JsonPropertyName("high")]
    public List<decimal?>? High { get; set; }

    [JsonPropertyName("low")]
    public List<decimal?>? Low { get; set; }

    [JsonPropertyName("close")]
    public List<decimal?>? Close { get; set; }

    [JsonPropertyName("volume")]
    public List<long?>? Volume { get; set; }
}

public sealed class ChartIndicatorAdjclose : YahooFinanceDto
{
    [JsonPropertyName("adjclose")]
    public List<decimal?>? Adjclose { get; set; }
}

public sealed class ChartEventsArray : YahooFinanceDto
{
    [JsonPropertyName("dividends")]
    public List<ChartEventDividend>? Dividends { get; set; }

    [JsonPropertyName("splits")]
    public List<ChartEventSplit>? Splits { get; set; }
}

public sealed class ChartEventsObject : YahooFinanceDto
{
    [JsonPropertyName("dividends")]
    public Dictionary<string, ChartEventDividend>? Dividends { get; set; }

    [JsonPropertyName("splits")]
    public Dictionary<string, ChartEventSplit>? Splits { get; set; }
}

public sealed class ChartEventDividend : YahooFinanceDto
{
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    [JsonPropertyName("date")]
    public long? Date { get; set; }
}

public sealed class ChartEventSplit : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public long? Date { get; set; }

    [JsonPropertyName("denominator")]
    public int? Denominator { get; set; }

    [JsonPropertyName("numerator")]
    public int? Numerator { get; set; }

    [JsonPropertyName("splitRatio")]
    public string? SplitRatio { get; set; }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// chart  (JSR: ChartResultArray / ChartResultObject + supporting interfaces)
// ------------------------------------------------------------

public sealed class ChartResultArray : YahooFinanceDto
{
    [JsonPropertyName("meta")]
    public ChartMeta Meta { get; set; } = null!;

    [JsonPropertyName("quotes")]
    public List<ChartResultArrayQuote> Quotes { get; set; } = [];

    [JsonPropertyName("events")]
    public ChartEventsArray? Events { get; set; }
}

public sealed class ChartResultArrayQuote : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

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
    public ChartMeta Meta { get; set; } = null!;

    [JsonPropertyName("timestamp")]
    public List<long>? Timestamp { get; set; }

    [JsonPropertyName("indicators")]
    public ChartIndicatorsObject Indicators { get; set; } = null!;

    [JsonPropertyName("events")]
    public ChartEventsObject? Events { get; set; }
}

public sealed class ChartMeta : YahooFinanceDto
{
    [JsonPropertyName("chartPreviousClose")]
    public decimal? ChartPreviousClose { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("currentTradingPeriod")]
    public ChartMetaCurrentTradingPeriod CurrentTradingPeriod { get; set; } = null!;

    [JsonPropertyName("dataGranularity")]
    public string DataGranularity { get; set; } = null!;

    [JsonPropertyName("exchangeName")]
    public string ExchangeName { get; set; } = null!;

    [JsonPropertyName("exchangeTimezoneName")]
    public string ExchangeTimezoneName { get; set; } = null!;

    [JsonPropertyName("fiftyTwoWeekHigh")]
    public decimal? FiftyTwoWeekHigh { get; set; }

    [JsonPropertyName("fiftyTwoWeekLow")]
    public decimal? FiftyTwoWeekLow { get; set; }

    [JsonPropertyName("firstTradeDate")]
    public DateTimeOffset? FirstTradeDate { get; set; }

    [JsonPropertyName("fullExchangeName")]
    public string? FullExchangeName { get; set; }

    [JsonPropertyName("gmtoffset")]
    public int Gmtoffset { get; set; }

    [JsonPropertyName("hasPrePostMarketData")]
    public bool? HasPrePostMarketData { get; set; }

    [JsonPropertyName("instrumentType")]
    public string InstrumentType { get; set; } = null!;

    [JsonPropertyName("longName")]
    public string? LongName { get; set; }

    [JsonPropertyName("previousClose")]
    public decimal? PreviousClose { get; set; }

    [JsonPropertyName("priceHint")]
    public int PriceHint { get; set; }

    [JsonPropertyName("range")]
    public string Range { get; set; } = null!;

    [JsonPropertyName("regularMarketDayHigh")]
    public decimal? RegularMarketDayHigh { get; set; }

    [JsonPropertyName("regularMarketDayLow")]
    public decimal? RegularMarketDayLow { get; set; }

    [JsonPropertyName("regularMarketPrice")]
    public decimal RegularMarketPrice { get; set; }

    [JsonPropertyName("regularMarketTime")]
    public DateTimeOffset RegularMarketTime { get; set; }

    [JsonPropertyName("regularMarketVolume")]
    public long? RegularMarketVolume { get; set; }

    [JsonPropertyName("scale")]
    public int? Scale { get; set; }

    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = null!;

    [JsonPropertyName("tradingPeriods")]
    [JsonConverter(typeof(ChartMetaTradingPeriodsValueConverter))]
    public ChartMetaTradingPeriodsValue? TradingPeriods { get; set; }

    [JsonPropertyName("validRanges")]
    public List<string> ValidRanges { get; set; } = [];
}

public sealed class ChartMetaCurrentTradingPeriod : YahooFinanceDto
{
    [JsonPropertyName("pre")]
    public ChartMetaTradingPeriod Pre { get; set; } = null!;

    [JsonPropertyName("regular")]
    public ChartMetaTradingPeriod Regular { get; set; } = null!;

    [JsonPropertyName("post")]
    public ChartMetaTradingPeriod Post { get; set; } = null!;
}

public sealed class ChartMetaTradingPeriod : YahooFinanceDto
{
    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = null!;

    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; set; }

    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    [JsonPropertyName("gmtoffset")]
    public int Gmtoffset { get; set; }
}

public sealed class ChartMetaTradingPeriods : YahooFinanceDto
{
    [JsonPropertyName("pre")]
    public List<List<ChartMetaTradingPeriod>>? Pre { get; set; }

    [JsonPropertyName("post")]
    public List<List<ChartMetaTradingPeriod>>? Post { get; set; }

    [JsonPropertyName("regular")]
    public List<List<ChartMetaTradingPeriod>>? Regular { get; set; }
}

public sealed class ChartMetaTradingPeriodsValue : YahooFinanceDto
{
    public ChartMetaTradingPeriods? Structured { get; set; }
    public List<List<ChartMetaTradingPeriod>>? Sessions { get; set; }
}

public sealed class ChartMetaTradingPeriodsValueConverter : JsonConverter<ChartMetaTradingPeriodsValue>
{
    public override ChartMetaTradingPeriodsValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            var structured = JsonSerializer.Deserialize<ChartMetaTradingPeriods>(ref reader, options);
            return new ChartMetaTradingPeriodsValue { Structured = structured };
        }

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var sessions = JsonSerializer.Deserialize<List<List<ChartMetaTradingPeriod>>>(ref reader, options);
            return new ChartMetaTradingPeriodsValue { Sessions = sessions };
        }

        throw new JsonException("Unexpected token when parsing tradingPeriods.");
    }

    public override void Write(Utf8JsonWriter writer, ChartMetaTradingPeriodsValue value, JsonSerializerOptions options)
    {
        if (value.Structured is not null)
        {
            JsonSerializer.Serialize(writer, value.Structured, options);
            return;
        }

        if (value.Sessions is not null)
        {
            JsonSerializer.Serialize(writer, value.Sessions, options);
            return;
        }

        writer.WriteNullValue();
    }
}

public sealed class ChartIndicatorsObject : YahooFinanceDto
{
    [JsonPropertyName("quote")]
    public List<ChartIndicatorQuote> Quote { get; set; } = [];

    [JsonPropertyName("adjclose")]
    public List<ChartIndicatorAdjclose>? Adjclose { get; set; }
}

public sealed class ChartIndicatorQuote : YahooFinanceDto
{
    [JsonPropertyName("open")]
    public List<decimal?> Open { get; set; } = [];

    [JsonPropertyName("high")]
    public List<decimal?> High { get; set; } = [];

    [JsonPropertyName("low")]
    public List<decimal?> Low { get; set; } = [];

    [JsonPropertyName("close")]
    public List<decimal?> Close { get; set; } = [];

    [JsonPropertyName("volume")]
    public List<long?> Volume { get; set; } = [];
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
    public decimal Amount { get; set; }

    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }
}

public sealed class ChartEventSplit : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("denominator")]
    public int Denominator { get; set; }

    [JsonPropertyName("numerator")]
    public int Numerator { get; set; }

    [JsonPropertyName("splitRatio")]
    public string SplitRatio { get; set; } = null!;
}

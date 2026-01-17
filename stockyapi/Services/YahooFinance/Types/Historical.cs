using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// historical  (JSR: result is arrays of HistoricalRow* depending on events)
// ------------------------------------------------------------

public sealed class HistoricalHistoryResult : List<HistoricalRowHistory> { }
public sealed class HistoricalDividendsResult : List<HistoricalRowDividend> { }
public sealed class HistoricalStockSplitsResult : List<HistoricalRowStockSplit> { }

public sealed class HistoricalRowHistory : YahooFinanceDto
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("open")]
    public decimal Open { get; set; }

    [JsonPropertyName("high")]
    public decimal High { get; set; }

    [JsonPropertyName("low")]
    public decimal Low { get; set; }

    [JsonPropertyName("close")]
    public decimal Close { get; set; }

    [JsonPropertyName("adjClose")]
    public decimal? AdjClose { get; set; }

    [JsonPropertyName("volume")]
    public long Volume { get; set; }
}

public sealed class HistoricalRowDividend : YahooFinanceDto
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("dividends")]
    public decimal Dividends { get; set; }
}

public sealed class HistoricalRowStockSplit : YahooFinanceDto
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("stockSplits")]
    public string StockSplits { get; set; } = null!;
}

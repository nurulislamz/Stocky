using System.Text.Json.Serialization;

namespace stockyapi.Repository.YahooFinance;

public class YahooFinanceResponse
{
    [JsonPropertyName("chart")]
    public Chart Chart { get; set; } = new();
}

public class Chart
{
    [JsonPropertyName("result")]
    public List<Data>? Data { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

public class Data
{
    [JsonPropertyName("meta")]
    public Meta Meta { get; set; } = new();

    [JsonPropertyName("timestamp")]
    public List<long>? Timestamp { get; set; }

    [JsonPropertyName("indicators")]
    public Indicators Indicators { get; set; } = new();
}

public class Meta
{
    [JsonPropertyName("regularMarketPrice")]
    public decimal RegularMarketPrice { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("regularMarketTime")]
    public long RegularMarketTime { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("chartPreviousClose")]
    public decimal ChartPreviousClose { get; set; }
}

public class Indicators
{
    [JsonPropertyName("quote")]
    public List<Quote>? Quote { get; set; }
}

public class Quote
{
    [JsonPropertyName("close")]
    public List<decimal?>? Close { get; set; }

    [JsonPropertyName("volume")]
    public List<decimal?>? Volume { get; set; }
}

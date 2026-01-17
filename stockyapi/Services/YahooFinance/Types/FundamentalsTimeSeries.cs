using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// fundamentalsTimeSeries  (JSR: time series for many financial types; huge surface)
// ------------------------------------------------------------

/// <summary>
/// fundamentalsTimeSeries() returns results keyed by type/period; the JSR type surface is very large.
/// Keep correct at the container level and safely preserve series content.
/// </summary>
public sealed class FundamentalsTimeSeriesResults : List<FundamentalsTimeSeriesResult> { }

public sealed class FundamentalsTimeSeriesResult : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("TYPE")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("periodType")]
    public string? PeriodType { get; set; }
}

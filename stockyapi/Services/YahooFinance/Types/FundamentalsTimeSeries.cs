using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// fundamentalsTimeSeries  (JSR: time series for many financial types; huge surface)
// ------------------------------------------------------------

/// <summary>
/// fundamentalsTimeSeries() returns results keyed by type/period; the JSR type surface is very large.
/// Keep correct at the container level and safely preserve series content.
/// </summary>
public sealed class FundamentalsTimeSeriesResults : YahooFinanceDto
{
    // yahoo-finance2 returns an object/array structure depending on options; keep flexible.
    // If you want to harden this later, we can map it once you capture a sample payload.
    [JsonPropertyName("timeseries")]
    public JsonElement? Timeseries { get; set; }
}

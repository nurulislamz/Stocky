using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// quoteSummary  (JSR: module-heavy; top-level result is "modules keyed by name")
// ------------------------------------------------------------

/// <summary>
/// yahoo-finance2 quoteSummary() returns an object whose keys are the requested modules.
/// The JSR page lists hundreds of module interfaces; this keeps the shape correct without exploding DTO count.
/// </summary>
public sealed class QuoteSummaryResult : YahooFinanceDto
{
    // Module payloads are keyed by module name (e.g. "assetProfile", "price", "summaryDetail"...)
    // Keep them typed later as you decide what you need.
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
}

using System.Text.Json;
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
    [JsonPropertyName("assetProfile")]
    public JsonElement? AssetProfile { get; set; }

    [JsonPropertyName("balanceSheetHistory")]
    public JsonElement? BalanceSheetHistory { get; set; }

    [JsonPropertyName("balanceSheetHistoryQuarterly")]
    public JsonElement? BalanceSheetHistoryQuarterly { get; set; }

    [JsonPropertyName("calendarEvents")]
    public JsonElement? CalendarEvents { get; set; }

    [JsonPropertyName("cashflowStatementHistory")]
    public JsonElement? CashflowStatementHistory { get; set; }

    [JsonPropertyName("cashflowStatementHistoryQuarterly")]
    public JsonElement? CashflowStatementHistoryQuarterly { get; set; }

    [JsonPropertyName("defaultKeyStatistics")]
    public JsonElement? DefaultKeyStatistics { get; set; }

    [JsonPropertyName("earnings")]
    public JsonElement? Earnings { get; set; }

    [JsonPropertyName("earningsHistory")]
    public JsonElement? EarningsHistory { get; set; }

    [JsonPropertyName("earningsTrend")]
    public JsonElement? EarningsTrend { get; set; }

    [JsonPropertyName("financialData")]
    public JsonElement? FinancialData { get; set; }

    [JsonPropertyName("fundOwnership")]
    public JsonElement? FundOwnership { get; set; }

    [JsonPropertyName("fundPerformance")]
    public JsonElement? FundPerformance { get; set; }

    [JsonPropertyName("fundProfile")]
    public JsonElement? FundProfile { get; set; }

    [JsonPropertyName("incomeStatementHistory")]
    public JsonElement? IncomeStatementHistory { get; set; }

    [JsonPropertyName("incomeStatementHistoryQuarterly")]
    public JsonElement? IncomeStatementHistoryQuarterly { get; set; }

    [JsonPropertyName("indexTrend")]
    public JsonElement? IndexTrend { get; set; }

    [JsonPropertyName("industryTrend")]
    public JsonElement? IndustryTrend { get; set; }

    [JsonPropertyName("insiderHolders")]
    public JsonElement? InsiderHolders { get; set; }

    [JsonPropertyName("insiderTransactions")]
    public JsonElement? InsiderTransactions { get; set; }

    [JsonPropertyName("institutionOwnership")]
    public JsonElement? InstitutionOwnership { get; set; }

    [JsonPropertyName("majorDirectHolders")]
    public JsonElement? MajorDirectHolders { get; set; }

    [JsonPropertyName("majorHoldersBreakdown")]
    public JsonElement? MajorHoldersBreakdown { get; set; }

    [JsonPropertyName("netSharePurchaseActivity")]
    public JsonElement? NetSharePurchaseActivity { get; set; }

    [JsonPropertyName("price")]
    public JsonElement? Price { get; set; }

    [JsonPropertyName("quoteType")]
    public JsonElement? QuoteType { get; set; }

    [JsonPropertyName("recommendationTrend")]
    public JsonElement? RecommendationTrend { get; set; }

    [JsonPropertyName("secFilings")]
    public JsonElement? SecFilings { get; set; }

    [JsonPropertyName("sectorTrend")]
    public JsonElement? SectorTrend { get; set; }

    [JsonPropertyName("summaryDetail")]
    public JsonElement? SummaryDetail { get; set; }

    [JsonPropertyName("summaryProfile")]
    public JsonElement? SummaryProfile { get; set; }

    [JsonPropertyName("topHoldings")]
    public JsonElement? TopHoldings { get; set; }

    [JsonPropertyName("upgradeDowngradeHistory")]
    public JsonElement? UpgradeDowngradeHistory { get; set; }
}

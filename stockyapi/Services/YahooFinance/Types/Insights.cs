using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// insights  (JSR: InsightsResult with nested interfaces)
// ------------------------------------------------------------

public sealed class InsightsResult : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("companySnapshot")]
    public InsightsCompanySnapshot? CompanySnapshot { get; set; }

    [JsonPropertyName("events")]
    public List<InsightsEvent>? Events { get; set; }

    [JsonPropertyName("instrumentInfo")]
    public InsightsInstrumentInfo? InstrumentInfo { get; set; }

    [JsonPropertyName("recommendation")]
    public JsonElement? Recommendation { get; set; } // structure varies

    [JsonPropertyName("reports")]
    public List<InsightsReport>? Reports { get; set; }

    [JsonPropertyName("secReports")]
    public List<InsightsSecReport>? SecReports { get; set; }

    [JsonPropertyName("sigDevs")]
    public List<InsightsSigDev>? SigDevs { get; set; }

    [JsonPropertyName("upsell")]
    public InsightsUpsell? Upsell { get; set; }

    [JsonPropertyName("upsellSearchDD")]
    public JsonElement? UpsellSearchDD { get; set; }
}

public sealed class InsightsCompanySnapshot : YahooFinanceDto
{
    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [JsonPropertyName("sector")]
    public string? Sector { get; set; }

    [JsonPropertyName("sectorInfo")]
    public string? SectorInfo { get; set; }
}

public sealed class InsightsInstrumentInfo : YahooFinanceDto
{
    [JsonPropertyName("keyTechnicals")]
    public JsonElement? KeyTechnicals { get; set; }

    [JsonPropertyName("technicalEvents")]
    public JsonElement? TechnicalEvents { get; set; }

    [JsonPropertyName("valuation")]
    public JsonElement? Valuation { get; set; }
}

public sealed class InsightsEvent : YahooFinanceDto
{
    [JsonPropertyName("startDate")]
    public long? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public long? EndDate { get; set; }

    [JsonPropertyName("eventType")]
    public string? EventType { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("pricePeriod")]
    public string? PricePeriod { get; set; }

    [JsonPropertyName("tradeType")]
    public string? TradeType { get; set; }

    [JsonPropertyName("tradingHorizon")]
    public string? TradingHorizon { get; set; }
}

public sealed class InsightsReport : YahooFinanceDto
{
    [JsonPropertyName("headHtml")]
    public string? HeadHtml { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("investmentRating")]
    public string? InvestmentRating { get; set; }

    [JsonPropertyName("provider")]
    public string? Provider { get; set; }

    [JsonPropertyName("reportDate")]
    public long? ReportDate { get; set; }

    [JsonPropertyName("reportTitle")]
    public string? ReportTitle { get; set; }

    [JsonPropertyName("reportType")]
    public string? ReportType { get; set; }

    [JsonPropertyName("targetPrice")]
    public decimal? TargetPrice { get; set; }

    [JsonPropertyName("targetPriceStatus")]
    public string? TargetPriceStatus { get; set; }

    [JsonPropertyName("tickers")]
    public List<string>? Tickers { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public sealed class InsightsSecReport : YahooFinanceDto
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("filingDate")]
    public long? FilingDate { get; set; }

    [JsonPropertyName("formType")]
    public string? FormType { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("snapshotUrl")]
    public string? SnapshotUrl { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public sealed class InsightsSigDev : YahooFinanceDto
{
    [JsonPropertyName("date")]
    public long? Date { get; set; }

    [JsonPropertyName("headline")]
    public string? Headline { get; set; }
}

public sealed class InsightsUpsell : YahooFinanceDto
{
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonPropertyName("msBearishSummary")]
    public string? MsBearishSummary { get; set; }

    [JsonPropertyName("msBullishBearishSummariesPublishDate")]
    public long? MsBullishBearishSummariesPublishDate { get; set; }

    [JsonPropertyName("msBullishSummary")]
    public string? MsBullishSummary { get; set; }

    [JsonPropertyName("upsellReportType")]
    public string? UpsellReportType { get; set; }
}

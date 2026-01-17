using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// insights  (JSR: InsightsResult with nested interfaces)
// ------------------------------------------------------------

public sealed class InsightsResult : YahooFinanceDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("companySnapshot")]
    public InsightsCompanySnapshot? CompanySnapshot { get; set; }

    [JsonPropertyName("events")]
    public List<InsightsEvent>? Events { get; set; }

    [JsonPropertyName("instrumentInfo")]
    public InsightsInstrumentInfo? InstrumentInfo { get; set; }

    [JsonPropertyName("recommendation")]
    public InsightsRecommendation? Recommendation { get; set; }

    [JsonPropertyName("reports")]
    public List<InsightsReport>? Reports { get; set; }

    [JsonPropertyName("secReports")]
    public List<InsightsSecReport>? SecReports { get; set; }

    [JsonPropertyName("sigDevs")]
    public List<InsightsSigDev> SigDevs { get; set; } = [];

    [JsonPropertyName("upsell")]
    public InsightsUpsell? Upsell { get; set; }

    [JsonPropertyName("upsellSearchDD")]
    public InsightsUpsellSearchDD? UpsellSearchDD { get; set; }
}

public sealed class InsightsCompanySnapshot : YahooFinanceDto
{
    [JsonPropertyName("sectorInfo")]
    public string? SectorInfo { get; set; }

    [JsonPropertyName("company")]
    public InsightsCompanySnapshotCompany Company { get; set; } = null!;

    [JsonPropertyName("sector")]
    public InsightsCompanySnapshotSector Sector { get; set; } = null!;
}

public sealed class InsightsCompanySnapshotCompany : YahooFinanceDto
{
    [JsonPropertyName("innovativeness")]
    public decimal? Innovativeness { get; set; }

    [JsonPropertyName("hiring")]
    public decimal? Hiring { get; set; }

    [JsonPropertyName("sustainability")]
    public decimal? Sustainability { get; set; }

    [JsonPropertyName("insiderSentiments")]
    public decimal? InsiderSentiments { get; set; }

    [JsonPropertyName("earningsReports")]
    public decimal? EarningsReports { get; set; }

    [JsonPropertyName("dividends")]
    public decimal? Dividends { get; set; }
}

public sealed class InsightsCompanySnapshotSector : YahooFinanceDto
{
    [JsonPropertyName("innovativeness")]
    public decimal Innovativeness { get; set; }

    [JsonPropertyName("hiring")]
    public decimal Hiring { get; set; }

    [JsonPropertyName("sustainability")]
    public decimal? Sustainability { get; set; }

    [JsonPropertyName("insiderSentiments")]
    public decimal InsiderSentiments { get; set; }

    [JsonPropertyName("earningsReports")]
    public decimal? EarningsReports { get; set; }

    [JsonPropertyName("dividends")]
    public decimal Dividends { get; set; }
}

public sealed class InsightsInstrumentInfo : YahooFinanceDto
{
    [JsonPropertyName("keyTechnicals")]
    public InsightsKeyTechnicals KeyTechnicals { get; set; } = null!;

    [JsonPropertyName("technicalEvents")]
    public InsightsTechnicalEvents TechnicalEvents { get; set; } = null!;

    [JsonPropertyName("valuation")]
    public InsightsValuation Valuation { get; set; } = null!;
}

public sealed class InsightsKeyTechnicals : YahooFinanceDto
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("support")]
    public decimal? Support { get; set; }

    [JsonPropertyName("resistance")]
    public decimal? Resistance { get; set; }

    [JsonPropertyName("stopLoss")]
    public decimal? StopLoss { get; set; }
}

public sealed class InsightsTechnicalEvents : YahooFinanceDto
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("sector")]
    public string? Sector { get; set; }

    [JsonPropertyName("shortTermOutlook")]
    public InsightsOutlook ShortTermOutlook { get; set; } = null!;

    [JsonPropertyName("intermediateTermOutlook")]
    public InsightsOutlook IntermediateTermOutlook { get; set; } = null!;

    [JsonPropertyName("longTermOutlook")]
    public InsightsOutlook LongTermOutlook { get; set; } = null!;
}

public sealed class InsightsValuation : YahooFinanceDto
{
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("color")]
    public int? Color { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("discount")]
    public string? Discount { get; set; }

    [JsonPropertyName("relativeValue")]
    public string? RelativeValue { get; set; }
}

public sealed class InsightsEvent : YahooFinanceDto
{
    [JsonPropertyName("startDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset StartDate { get; set; }

    [JsonPropertyName("endDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset EndDate { get; set; }

    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = null!;

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = null!;

    [JsonPropertyName("pricePeriod")]
    public string PricePeriod { get; set; } = null!;

    [JsonPropertyName("tradeType")]
    public string TradeType { get; set; } = null!;

    [JsonPropertyName("tradingHorizon")]
    public string TradingHorizon { get; set; } = null!;
}

public sealed class InsightsReport : YahooFinanceDto
{
    [JsonPropertyName("headHtml")]
    public string HeadHtml { get; set; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("investmentRating")]
    public string? InvestmentRating { get; set; }

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("reportDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset ReportDate { get; set; }

    [JsonPropertyName("reportTitle")]
    public string ReportTitle { get; set; } = null!;

    [JsonPropertyName("reportType")]
    public string ReportType { get; set; } = null!;

    [JsonPropertyName("targetPrice")]
    public decimal? TargetPrice { get; set; }

    [JsonPropertyName("targetPriceStatus")]
    public string? TargetPriceStatus { get; set; }

    [JsonPropertyName("tickers")]
    public List<string>? Tickers { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public sealed class InsightsResearchReport : YahooFinanceDto
{
    [JsonPropertyName("reportId")]
    public string ReportId { get; set; } = null!;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("reportDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset ReportDate { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = null!;

    [JsonPropertyName("investmentRating")]
    public string? InvestmentRating { get; set; }
}

public sealed class InsightsSecReport : YahooFinanceDto
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("filingDate")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset FilingDate { get; set; }

    [JsonPropertyName("formType")]
    public string FormType { get; set; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("snapshotUrl")]
    public string SnapshotUrl { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
}

public sealed class InsightsSigDev : YahooFinanceDto
{
    [JsonPropertyName("date")]
    [JsonConverter(typeof(UnixSecondsDateTimeOffsetConverter))]
    public DateTimeOffset Date { get; set; }

    [JsonPropertyName("headline")]
    public string Headline { get; set; } = null!;
}

public sealed class InsightsOutlook : YahooFinanceDto
{
    [JsonPropertyName("stateDescription")]
    public string StateDescription { get; set; } = null!;

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = null!;

    [JsonPropertyName("score")]
    public decimal Score { get; set; }

    [JsonPropertyName("scoreDescription")]
    public string ScoreDescription { get; set; } = null!;

    [JsonPropertyName("sectorDirection")]
    public string? SectorDirection { get; set; }

    [JsonPropertyName("sectorScore")]
    public decimal? SectorScore { get; set; }

    [JsonPropertyName("sectorScoreDescription")]
    public string? SectorScoreDescription { get; set; }

    [JsonPropertyName("indexDirection")]
    public string IndexDirection { get; set; } = null!;

    [JsonPropertyName("indexScore")]
    public decimal IndexScore { get; set; }

    [JsonPropertyName("indexScoreDescription")]
    public string IndexScoreDescription { get; set; } = null!;
}

public sealed class InsightsUpsell : YahooFinanceDto
{
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    [JsonPropertyName("msBearishSummary")]
    public List<string>? MsBearishSummary { get; set; }

    [JsonPropertyName("msBullishBearishSummariesPublishDate")]
    [JsonConverter(typeof(UnixSecondsNullableDateTimeOffsetConverter))]
    public DateTimeOffset? MsBullishBearishSummariesPublishDate { get; set; }

    [JsonPropertyName("msBullishSummary")]
    public List<string>? MsBullishSummary { get; set; }

    [JsonPropertyName("upsellReportType")]
    public string? UpsellReportType { get; set; }
}

public sealed class InsightsUpsellSearchDD : YahooFinanceDto
{
    [JsonPropertyName("researchReports")]
    public InsightsResearchReport ResearchReports { get; set; } = null!;
}

public sealed class InsightsRecommendation : YahooFinanceDto
{
    [JsonPropertyName("targetPrice")]
    public decimal? TargetPrice { get; set; }

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = null!;

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = null!;
}

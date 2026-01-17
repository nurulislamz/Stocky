using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// search  (JSR: SearchResult with lots of timing fields + union quotes + news)
// ------------------------------------------------------------

public sealed class SearchResult : YahooFinanceDto
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("quotes")]
    public List<SearchQuote> Quotes { get; set; } = [];

    [JsonPropertyName("news")]
    public List<SearchNews> News { get; set; } = [];

    [JsonPropertyName("explains")]
    public List<object> Explains { get; set; } = [];

    [JsonPropertyName("nav")]
    public List<object> Nav { get; set; } = [];

    [JsonPropertyName("lists")]
    public List<object> Lists { get; set; } = [];

    [JsonPropertyName("researchReports")]
    public List<object> ResearchReports { get; set; } = [];

    [JsonPropertyName("screenerFieldResults")]
    public List<object>? ScreenerFieldResults { get; set; }

    [JsonPropertyName("culturalAssets")]
    public List<object>? CulturalAssets { get; set; }

    // performance timing fields (repo lists many)
    [JsonPropertyName("totalTime")]
    public int TotalTime { get; set; }

    [JsonPropertyName("timeTakenForQuotes")]
    public int TimeTakenForQuotes { get; set; }

    [JsonPropertyName("timeTakenForNews")]
    public int TimeTakenForNews { get; set; }

    [JsonPropertyName("timeTakenForAlgowatchlist")]
    public int? TimeTakenForAlgowatchlist { get; set; }

    [JsonPropertyName("timeTakenForPredefinedScreener")]
    public int? TimeTakenForPredefinedScreener { get; set; }

    [JsonPropertyName("timeTakenForCrunchbase")]
    public int? TimeTakenForCrunchbase { get; set; }

    [JsonPropertyName("timeTakenForNav")]
    public int? TimeTakenForNav { get; set; }

    [JsonPropertyName("timeTakenForResearchReports")]
    public int? TimeTakenForResearchReports { get; set; }

    [JsonPropertyName("timeTakenForScreenerField")]
    public int? TimeTakenForScreenerField { get; set; }

    [JsonPropertyName("timeTakenForCulturalAssets")]
    public int? TimeTakenForCulturalAssets { get; set; }

    [JsonPropertyName("timeTakenForSearchLists")]
    public int? TimeTakenForSearchLists { get; set; }
}

public sealed class SearchNews : YahooFinanceDto
{
    [JsonPropertyName("link")]
    public string Link { get; set; } = null!;

    [JsonPropertyName("providerPublishTime")]
    public DateTimeOffset ProviderPublishTime { get; set; }

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = null!;

    [JsonPropertyName("relatedTickers")]
    public List<string>? RelatedTickers { get; set; }

    [JsonPropertyName("thumbnail")]
    public SearchNewsThumbnail? Thumbnail { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("uuid")]
    public string Uuid { get; set; } = null!;
}

public sealed class SearchNewsThumbnail : YahooFinanceDto
{
    [JsonPropertyName("resolutions")]
    public List<SearchNewsThumbnailResolution> Resolutions { get; set; } = [];
}

public sealed class SearchNewsThumbnailResolution : YahooFinanceDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = null!;
}

/// <summary>
/// JSR: SearchQuote is a union of SearchQuoteYahoo* and SearchQuoteNonYahoo.
/// Model common fields + keep union specifics in Extra.
/// </summary>
public sealed class SearchQuote : YahooFinanceDto
{
    [JsonPropertyName("index")]
    public string? Index { get; set; }

    [JsonPropertyName("isYahooFinance")]
    public bool? IsYahooFinance { get; set; }

    [JsonPropertyName("score")]
    public decimal? Score { get; set; }

    [JsonPropertyName("newListingDate")]
    public DateTimeOffset? NewListingDate { get; set; }

    [JsonPropertyName("prevName")]
    public string? PrevName { get; set; }

    [JsonPropertyName("nameChangeDate")]
    public DateTimeOffset? NameChangeDate { get; set; }

    [JsonPropertyName("sector")]
    public string? Sector { get; set; }

    [JsonPropertyName("industry")]
    public string? Industry { get; set; }

    [JsonPropertyName("dispSecIndFlag")]
    public bool? DispSecIndFlag { get; set; }

    // Yahoo quotes:
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }

    [JsonPropertyName("shortname")]
    public string? Shortname { get; set; }

    [JsonPropertyName("longname")]
    public string? Longname { get; set; }

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("exchDisp")]
    public string? ExchDisp { get; set; }

    [JsonPropertyName("quoteType")]
    public string? QuoteType { get; set; }

    [JsonPropertyName("typeDisp")]
    public string? TypeDisp { get; set; }

    [JsonPropertyName("sectorDisp")]
    public string? SectorDisp { get; set; }

    [JsonPropertyName("industryDisp")]
    public string? IndustryDisp { get; set; }

    // Non-Yahoo entities:
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }
}

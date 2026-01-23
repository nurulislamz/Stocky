using System.Text.Json;
using System.Text.Json.Serialization;

namespace stockyapi.Services.YahooFinance.Types;

// ------------------------------------------------------------
// search  (JSR: SearchResult with lots of timing fields + union quotes + news)
// ------------------------------------------------------------

public sealed class SearchResult : YahooFinanceDto
{
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    [JsonPropertyName("quotes")]
    public List<SearchQuote> Quotes { get; set; } = [];

    [JsonPropertyName("news")]
    public List<SearchNews>? News { get; set; }

    [JsonPropertyName("nav")]
    public JsonElement? Nav { get; set; }

    [JsonPropertyName("lists")]
    public JsonElement? Lists { get; set; }

    [JsonPropertyName("researchReports")]
    public JsonElement? ResearchReports { get; set; }

    // performance timing fields (repo lists many)
    [JsonPropertyName("totalTime")]
    public int? TotalTime { get; set; }

    [JsonPropertyName("timeTakenForQuotes")]
    public int? TimeTakenForQuotes { get; set; }

    [JsonPropertyName("timeTakenForNews")]
    public int? TimeTakenForNews { get; set; }
}

public sealed class SearchNews : YahooFinanceDto
{
    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("providerPublishTime")]
    public long? ProviderPublishTime { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("relatedTickers")]
    public List<string>? RelatedTickers { get; set; }

    [JsonPropertyName("thumbnail")]
    public JsonElement? Thumbnail { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
}

/// <summary>
/// JSR: SearchQuote is a union of SearchQuoteYahoo* and SearchQuoteNonYahoo.
/// Model common fields + keep union specifics in Extra.
/// </summary>
public sealed class SearchQuote : YahooFinanceDto
{
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    [JsonPropertyName("isYahooFinance")]
    public bool? IsYahooFinance { get; set; }

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

    // Non-Yahoo entities:
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("permalink")]
    public string? Permalink { get; set; }
}

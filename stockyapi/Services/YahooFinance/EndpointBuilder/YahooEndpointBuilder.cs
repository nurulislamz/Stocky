using System.Text;

namespace stockyapi.Services.YahooFinance.EndpointBuilder;

public static class YahooEndpointBuilder
{
    private const string Query1Host = "https://query1.finance.yahoo.com";
    private const string Query2Host = "https://query2.finance.yahoo.com";
    
    public static Uri BuildCrumb()
    {
        return BuildUri(
            Query2Host,
            "/v1/test/getcrumb"
        );
    }

    public static Uri BuildChartUri(string symbol, YahooRange range, YahooInterval interval, YahooFields[] fields)
    {
        return BuildUri(
            Query1Host,
            $"/v8/finance/chart/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["range"] = range.ToApiString(),
                ["interval"] = interval.ToApiString(),
                ["fields"] = fields != null ? string.Join(',', fields) : null
            }
        );
    }

    public static Uri BuildFundamentalsTimeSeriesUri(string symbol, params string[] types)
    {
        return BuildUri(
            Query1Host,
            $"/ws/fundamentals-timeseries/v1/finance/timeseries/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["type"] = types != null ? string.Join(",", types) : null
            }
        );
    }

    public static Uri BuildHistoricalUri(string symbol, DateTime period1, DateTime period2, string interval)
    {
        return BuildUri(
            Query1Host,
            $"/v8/finance/chart/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["period1"] = ToUnixTimestamp(period1),
                ["period2"] = ToUnixTimestamp(period2),
                ["interval"] = interval
            }
        );
    }

    public static Uri BuildInsightsUri(string symbol)
    {
        return BuildUri(
            Query1Host,
            "/ws/insights/v2/finance/insights",
            new Dictionary<string, string?>
            {
                ["symbol"] = symbol
            }
        );
    }

    public static Uri BuildOptionsUri(string symbol, string? date = null)
    {
        return BuildUri(
            Query1Host,
            $"/v7/finance/options/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["date"] = string.IsNullOrWhiteSpace(date) ? null : date
            }
        );
    }

    public static Uri BuildQuoteUri(params string[] symbols)
    {
        return BuildUri(
            Query2Host,
            "/v7/finance/quote",
            new Dictionary<string, string?>
            {
                ["symbols"] = symbols != null ? string.Join(",", symbols) : null
            }
        );
    }

    public static Uri BuildQuoteSummaryUri(string symbol, params string[] modules)
    {
        return BuildUri(
            Query1Host,
            $"/v10/finance/quoteSummary/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["modules"] = modules != null ? string.Join(",", modules) : null
            }
        );
    }

    public static Uri BuildRecommendationsBySymbolUri(string symbol)
    {
        return BuildUri(
            Query1Host,
            $"/v6/finance/recommendationsbysymbol/{EscapePathSegment(symbol)}"
        );
    }

    public static Uri BuildScreenerUri(string screenerId)
    {
        return BuildUri(
            Query1Host,
            "/v1/finance/screener/predefined/saved",
            new Dictionary<string, string?>
            {
                ["scrIds"] = screenerId
            }
        );
    }

    public static Uri BuildSearchUri(string query)
    {
        return BuildUri(
            Query1Host,
            "/v1/finance/search",
            new Dictionary<string, string?>
            {
                ["q"] = query
            }
        );
    }

    public static Uri BuildTrendingSymbolsUri(string region)
    {
        return BuildUri(
            Query1Host,
            $"/v1/finance/trending/{EscapePathSegment(region)}"
        );
    }

    public static Uri BuildDownloadCsvUri(string symbol, DateTime period1, DateTime period2)
    {
        return BuildUri(
            Query1Host,
            $"/v7/finance/download/{EscapePathSegment(symbol)}",
            new Dictionary<string, string?>
            {
                ["period1"] = ToUnixTimestamp(period1),
                ["period2"] = ToUnixTimestamp(period2),
                ["interval"] = "1d",
                ["events"] = "history",
                ["includeAdjustedClose"] = "true"
            }
        );
    }

    public static Uri BuildGetCrumbUri()
        => BuildUri(Query1Host, "/v1/test/getcrumb");

    private static Uri BuildUri(string host, string path, IReadOnlyDictionary<string, string?>? queryParams = null)
    {
        var builder = new UriBuilder(host)
        {
            Path = path
        };

        if (queryParams is not null)
        {
            var query = BuildQueryString(queryParams);
            if (!string.IsNullOrEmpty(query))
            {
                builder.Query = query;
            }
        }

        return builder.Uri;
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string?> queryParams)
    {
        var builder = new StringBuilder();
        foreach (var (key, value) in queryParams)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.Append('&');
            }

            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }

        return builder.ToString();
    }

    private static string EscapePathSegment(string? value) => Uri.EscapeDataString(value ?? string.Empty);

    private static string ToUnixTimestamp(DateTime date)
    {
        return new DateTimeOffset(date).ToUnixTimeSeconds().ToString();
    }
}

using System.Text;
using stockyapi.Middleware;

namespace stockyapi.Services.YahooFinance.Types;

public static class YahooEndpointBuilder
{
    private const string Query1Host = "https://query1.finance.yahoo.com";
    private const string Query2Host = "https://query2.finance.yahoo.com";

    public static Uri BuildChartUri(string symbol, YahooRange range, YahooInterval interval, YahooFields[] fields)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        return BuildUri(
            Query1Host,
            $"/v8/finance/chart/{EscapePathSegment(symbolValue)}",
            new Dictionary<string, string?>
            {
                ["range"] = range.ToApiString(),
                ["interval"] = interval.ToApiString(),
                ["fields"] = string.Join(',', nameof(fields))
            }
        );
    }

    public static Uri BuildFundamentalsTimeSeriesUri(string symbol, params string[] types)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        var typesValue = RequireCsv(types, nameof(types));
        return BuildUri(
            Query1Host,
            $"/ws/fundamentals-timeseries/v1/finance/timeseries/{EscapePathSegment(symbolValue)}",
            new Dictionary<string, string?>
            {
                ["type"] = typesValue
            }
        );
    }

    public static Uri BuildHistoricalUri(string symbol, string period1, string period2, string interval)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        var period1Value = RequireValue(period1, nameof(period1));
        var period2Value = RequireValue(period2, nameof(period2));
        var intervalValue = RequireValue(interval, nameof(interval));
        return BuildUri(
            Query1Host,
            $"/v8/finance/chart/{EscapePathSegment(symbolValue)}",
            new Dictionary<string, string?>
            {
                ["period1"] = period1Value,
                ["period2"] = period2Value,
                ["interval"] = intervalValue
            }
        );
    }

    public static Uri BuildInsightsUri(string symbol)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        return BuildUri(
            Query1Host,
            "/ws/insights/v2/finance/insights",
            new Dictionary<string, string?>
            {
                ["symbol"] = symbolValue
            }
        );
    }

    public static Uri BuildOptionsUri(string symbol, string? date = null)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        return BuildUri(
            Query1Host,
            $"/v7/finance/options/{EscapePathSegment(symbolValue)}",
            new Dictionary<string, string?>
            {
                ["date"] = string.IsNullOrWhiteSpace(date) ? null : date
            }
        );
    }

    public static Uri BuildQuoteUri(params string[] symbols)
    {
        var symbolsValue = RequireCsv(symbols, nameof(symbols));
        return BuildUri(
            Query2Host,
            "/v7/finance/quote",
            new Dictionary<string, string?>
            {
                ["symbols"] = symbolsValue
            }
        );
    }

    public static Uri BuildQuoteSummaryUri(string symbol, params string[] modules)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        var modulesValue = RequireCsv(modules, nameof(modules));
        return BuildUri(
            Query1Host,
            $"/v10/finance/quoteSummary/{EscapePathSegment(symbolValue)}",
            new Dictionary<string, string?>
            {
                ["modules"] = modulesValue
            }
        );
    }

    public static Uri BuildRecommendationsBySymbolUri(string symbol)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        return BuildUri(
            Query1Host,
            $"/v6/finance/recommendationsbysymbol/{EscapePathSegment(symbolValue)}"
        );
    }

    public static Uri BuildScreenerUri(string screenerId)
    {
        var screenerValue = RequireValue(screenerId, nameof(screenerId));
        return BuildUri(
            Query1Host,
            "/v1/finance/screener/predefined/saved",
            new Dictionary<string, string?>
            {
                ["scrIds"] = screenerValue
            }
        );
    }

    public static Uri BuildSearchUri(string query)
    {
        var queryValue = RequireValue(query, nameof(query));
        return BuildUri(
            Query1Host,
            "/v1/finance/search",
            new Dictionary<string, string?>
            {
                ["q"] = queryValue
            }
        );
    }

    public static Uri BuildTrendingSymbolsUri(string region)
    {
        var regionValue = RequireValue(region, nameof(region));
        return BuildUri(
            Query1Host,
            $"/v1/finance/trending/{EscapePathSegment(regionValue)}"
        );
    }

    public static Uri BuildDownloadCsvUri(string symbol)
    {
        var symbolValue = RequireValue(symbol, nameof(symbol));
        return BuildUri(
            Query1Host,
            $"/v7/finance/download/{EscapePathSegment(symbolValue)}"
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

    private static string RequireValue(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        return value;
    }

    private static string RequireCsv(IEnumerable<string>? values, string paramName)
    {
        if (values is null)
        {
            throw new ArgumentNullException(paramName);
        }

        var trimmed = values
            .Select(value => value?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();

        if (trimmed.Count == 0)
        {
            throw new ArgumentException("At least one value is required.", paramName);
        }

        return string.Join(",", trimmed);
    }

    private static string EscapePathSegment(string value) => Uri.EscapeDataString(value);
}

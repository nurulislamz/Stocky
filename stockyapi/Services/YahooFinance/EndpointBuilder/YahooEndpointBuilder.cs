using System.Text;
namespace stockyapi.Services.YahooFinance.Types;

public static class YahooEndpointBuilder
{
    private const string Query1 = "https://query1.finance.yahoo.com";
    private const string Query2 = "https://query2.finance.yahoo.com";

    public static Uri Build(YahooEndpoint endpoint, string? symbol = null, YahooRange? range = null, YahooInterval? interval = null, string? region = null)
    {
        var (host, path) = endpoint switch
        {
            YahooEndpoint.Quote => (Query2, "/v7/finance/quote"),
            YahooEndpoint.Chart => (Query1, "/v8/finance/chart/{symbol}"),
            YahooEndpoint.Options => (Query1, "/v7/finance/options/{symbol}"),
            YahooEndpoint.Search => (Query1, "/v1/finance/search"),
            YahooEndpoint.QuoteSummary => (Query1, "/v10/finance/quoteSummary/{symbol}"),
            YahooEndpoint.FundamentalsTimeSeries => (Query1, "/ws/fundamentals-timeseries/v1/finance/timeseries/{symbol}"),
            YahooEndpoint.TrendingSymbols => (Query1, "/v1/finance/trending/{region}"),
            YahooEndpoint.ScreenerPredefined => (Query1, "/v1/finance/screener/predefined/saved"),
            YahooEndpoint.RecommendationsBySymbol => (Query1, "/v6/finance/recommendationsbysymbol/{symbol}"),
            YahooEndpoint.DownloadCsv => (Query1, "/v7/finance/download/{symbol}"),
            YahooEndpoint.GetCrumb => (Query1, "/v1/test/getcrumb"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null)
        };

        var sb = new StringBuilder(host);
        sb.Append(path);

        // Path Parameters
        if (symbol is not null)
            sb.Replace("{symbol}", Uri.EscapeDataString(symbol));

        if (region is not null)
            sb.Replace("{region}", Uri.EscapeDataString(region));

        // Query Parameters
        char separator = '?';

        if (range.HasValue)
        {
            sb.Append(separator).Append("range=").Append(range.Value.ToApiString());
            separator = '&';
        }

        if (interval.HasValue)
        {
            sb.Append(separator).Append("interval=").Append(interval.Value.ToApiString());
            separator = '&';
        }

        return new Uri(sb.ToString());
    }
    

}

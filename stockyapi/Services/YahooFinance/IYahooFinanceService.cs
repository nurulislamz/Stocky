using stockyapi.Middleware;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Services.YahooFinance;

public interface IYahooFinanceService
{
    Task<Result<string>> GetCrumb(CancellationToken cancellationToken);
    
    Task<Result<YahooChartResponse>> GetChartAsync(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        YahooFields[] fields,
        CancellationToken cancellationToken = default
    );

    Task<Result<FundamentalsTimeSeriesResponse>> GetFundamentalsTimeSeriesAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] types
    );

    Task<Result<HistoricalHistoryResult>> GetHistoricalAsync(
        string symbol,
        DateTime period1,
        DateTime period2,
        YahooInterval interval,
        CancellationToken cancellationToken = default
    );

    Task<Result<InsightsApiResponse>> GetInsightsAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<OptionsApiResponse>> GetOptionsAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<OptionsApiResponse>> GetOptionsAsync(
        string symbol,
        string date,
        CancellationToken cancellationToken = default
    );

    [Obsolete("Yahoo Finance no longer supports Quotes", true)]
    Task<Result<QuoteResponseArray>> GetQuoteAsync(
        CancellationToken cancellationToken = default,
        params string[] symbols
    );

    [Obsolete("Yahoo Finance no longer supports Quotes", true)]
    Task<Result<QuoteSummaryResult>> GetQuoteSummaryAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] modules
    );

    Task<Result<RecommendationsBySymbolApiResponse>> GetRecommendationsBySymbolAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<ScreenerResponse>> RunScreenerAsync(
        string screenerId,
        CancellationToken cancellationToken = default
    );

    Task<Result<SearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    );

    Task<Result<TrendingSymbolsResponse>> GetTrendingSymbolsAsync(
        string region,
        CancellationToken cancellationToken = default
    );
}
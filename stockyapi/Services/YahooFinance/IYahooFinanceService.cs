using stockyapi.Middleware;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Services.YahooFinance;

public interface IYahooFinanceService
{
    Task<Result<ChartResultArray>> GetChartAsync(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        YahooFields[] fields,
        CancellationToken cancellationToken = default
    );

    Task<Result<FundamentalsTimeSeriesResults>> GetFundamentalsTimeSeriesAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] types
    );

    Task<Result<HistoricalHistoryResult>> GetHistoricalAsync(
        string symbol,
        string period1,
        string period2,
        string interval,
        CancellationToken cancellationToken = default
    );

    Task<Result<InsightsResult>> GetInsightsAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        string date,
        CancellationToken cancellationToken = default
    );

    Task<Result<QuoteResponseArray>> GetQuoteAsync(
        CancellationToken cancellationToken = default,
        params string[] symbols
    );

    Task<Result<QuoteSummaryResult>> GetQuoteSummaryAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] modules
    );

    Task<Result<RecommendationsBySymbolResponseArray>> GetRecommendationsBySymbolAsync(
        string symbol,
        CancellationToken cancellationToken = default
    );

    Task<Result<ScreenerResult>> RunScreenerAsync(
        string screenerId,
        CancellationToken cancellationToken = default
    );

    Task<Result<SearchResult>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default
    );

    Task<Result<TrendingSymbolsResult>> GetTrendingSymbolsAsync(
        string region,
        CancellationToken cancellationToken = default
    );
}

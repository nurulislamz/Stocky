using stockyapi.Middleware;
using stockyapi.Repository.YahooFinance.Helper;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Repository.YahooFinance;

public sealed class YahooFinanceService : IYahooFinanceService
{
    private readonly YahooExecutionHelper _executor;

    public YahooFinanceService(YahooExecutionHelper executor)
    {
        _executor = executor;
    }

    public Task<Result<ChartResultArray>> GetChartAsync(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<ChartResultArray>(
            cacheKey: $"chart:{symbol}:{range}:{interval}",
            cacheTtl: TimeSpan.FromMinutes(10),
            uri: YahooEndpointBuilder.BuildChartUri(symbol, range, interval),
            ct
        );
    }

    public Task<Result<FundamentalsTimeSeriesResults>> GetFundamentalsTimeSeriesAsync(
        string symbol,
        CancellationToken ct = default,
        params string[] types)
    {
        return _executor.ExecuteAsync<FundamentalsTimeSeriesResults>(
            cacheKey: $"fundamentals:{symbol}:{string.Join(",", types)}",
            cacheTtl: TimeSpan.FromHours(1),
            uri: YahooEndpointBuilder.BuildFundamentalsTimeSeriesUri(symbol, types),
            ct
        );
    }

    public Task<Result<HistoricalHistoryResult>> GetHistoricalAsync(
        string symbol,
        string period1,
        string period2,
        string interval,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<HistoricalHistoryResult>(
            cacheKey: $"historical:{symbol}:{period1}:{period2}:{interval}",
            cacheTtl: TimeSpan.FromHours(6),
            uri: YahooEndpointBuilder.BuildHistoricalUri(symbol, period1, period2, interval),
            ct
        );
    }

    public Task<Result<InsightsResult>> GetInsightsAsync(
        string symbol,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<InsightsResult>(
            cacheKey: $"insights:{symbol}",
            cacheTtl: TimeSpan.FromHours(12),
            uri: YahooEndpointBuilder.BuildInsightsUri(symbol),
            ct
        );
    }

    public Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        CancellationToken ct = default)
        => GetOptionsAsync(symbol, null, ct);

    public Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        string? date,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<OptionsResult>(
            cacheKey: $"options:{symbol}:{date ?? "latest"}",
            cacheTtl: TimeSpan.FromMinutes(15),
            uri: YahooEndpointBuilder.BuildOptionsUri(symbol, date),
            ct
        );
    }

    public Task<Result<QuoteResponseArray>> GetQuoteAsync(
        CancellationToken ct = default,
        params string[] symbols)
    {
        return _executor.ExecuteAsync<QuoteResponseArray>(
            cacheKey: $"quote:{string.Join(",", symbols)}",
            cacheTtl: TimeSpan.FromSeconds(30),
            uri: YahooEndpointBuilder.BuildQuoteUri(symbols),
            ct
        );
    }

    public Task<Result<QuoteSummaryResult>> GetQuoteSummaryAsync(
        string symbol,
        CancellationToken ct = default,
        params string[] modules)
    {
        return _executor.ExecuteAsync<QuoteSummaryResult>(
            cacheKey: $"quotesummary:{symbol}:{string.Join(",", modules)}",
            cacheTtl: TimeSpan.FromHours(2),
            uri: YahooEndpointBuilder.BuildQuoteSummaryUri(symbol, modules),
            ct
        );
    }

    public Task<Result<RecommendationsBySymbolResponseArray>> GetRecommendationsBySymbolAsync(
        string symbol,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<RecommendationsBySymbolResponseArray>(
            cacheKey: $"recommendations:{symbol}",
            cacheTtl: TimeSpan.FromHours(6),
            uri: YahooEndpointBuilder.BuildRecommendationsBySymbolUri(symbol),
            ct
        );
    }

    public Task<Result<ScreenerResult>> RunScreenerAsync(
        string screenerId,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<ScreenerResult>(
            cacheKey: $"screener:{screenerId}",
            cacheTtl: TimeSpan.FromMinutes(5),
            uri: YahooEndpointBuilder.BuildScreenerUri(screenerId),
            ct
        );
    }

    public Task<Result<SearchResult>> SearchAsync(
        string query,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<SearchResult>(
            cacheKey: $"search:{query}",
            cacheTtl: TimeSpan.FromMinutes(10),
            uri: YahooEndpointBuilder.BuildSearchUri(query),
            ct
        );
    }

    public Task<Result<TrendingSymbolsResult>> GetTrendingSymbolsAsync(
        string region,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<TrendingSymbolsResult>(
            cacheKey: $"trending:{region}",
            cacheTtl: TimeSpan.FromMinutes(10),
            uri: YahooEndpointBuilder.BuildTrendingSymbolsUri(region),
            ct
        );
    }
}

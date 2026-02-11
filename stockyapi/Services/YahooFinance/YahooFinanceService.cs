using stockyapi.Middleware;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Helper;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Services.YahooFinance;

/// <summary>
/// Provides access to Yahoo Finance endpoints such as charts, quotes,
/// fundamentals, options, and market data.
/// 
/// This service acts as a thin abstraction over Yahoo Finance APIs and
/// applies caching and execution policies via <see cref="BaseApiServiceClient"/>.
/// </summary>
public sealed class YahooFinanceService : IYahooFinanceService
{
    private readonly BaseApiServiceClient _executor;

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooFinanceService"/> class.
    /// </summary>
    /// <param name="executor">
    /// Execution helper responsible for HTTP requests, caching, and error handling.
    /// </param>
    public YahooFinanceService(BaseApiServiceClient executor)
    {
        _executor = executor;
    }

    public async Task<Result<string>> GetCrumb(CancellationToken ct)
    {
        return await _executor.ExecuteAsync<string>(
            cacheKey: $"crumb",
            cacheTtl: TimeSpan.FromMinutes(10),
            uri: YahooEndpointBuilder.BuildCrumb(),
            ct
        );
    }

    /// <summary>
    /// Retrieves historical price chart data for a given symbol.
    /// </summary>
    /// <remarks>
    /// Maps to Yahoo Finance <c>/chart</c> endpoint.
    /// Supports different time ranges and intervals such as 1d, 5d, 1mo, 1y, etc.
    /// Results are cached for 10 minutes.
    /// </remarks>
    /// <param name="symbol">Ticker symbol (e.g. AAPL, MSFT).</param>
    /// <param name="range">Time range for the chart data.</param>
    /// <param name="interval">Data granularity interval.</param>
    /// <param name="fields">The fields to include in the response.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing chart price and volume data.
    /// </returns>
    public Task<Result<ChartResultArray>> GetChartAsync(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        YahooFields[] fields,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<ChartResultArray>(
            cacheKey: $"chart:{symbol}:{range}:{interval}",
            cacheTtl: TimeSpan.FromMinutes(10),
            uri: YahooEndpointBuilder.BuildChartUri(symbol, range, interval, fields),
            ct
        );
    }

    /// <summary>
    /// Retrieves time-series fundamental data for a given symbol.
    /// </summary>
    /// <remarks>
    /// Maps to Yahoo Finance <c>/fundamentals-timeseries</c> endpoint.
    /// Used for metrics such as revenue, EPS, cash flow, etc.
    /// Results are cached for 1 hour.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="types">
    /// One or more fundamental metric identifiers (e.g. annualRevenue, quarterlyEPS).
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing time-series fundamentals.
    /// </returns>
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

    /// <summary>
    /// Retrieves raw historical price data for a symbol within a specified time window.
    /// </summary>
    /// <remarks>
    /// Maps to Yahoo Finance <c>/v7/finance/download</c> or historical chart endpoints.
    /// Results are cached for 6 hours.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="period1">Start timestamp (Unix epoch seconds).</param>
    /// <param name="period2">End timestamp (Unix epoch seconds).</param>
    /// <param name="interval">Data interval (e.g. 1d, 1wk).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing historical price records.
    /// </returns>
    public Task<Result<HistoricalHistoryResult>> GetHistoricalAsync(
        string symbol,
        DateTime period1,
        DateTime period2,
        YahooInterval interval,
        CancellationToken ct = default)
    {
        return _executor.ExecuteAsync<HistoricalHistoryResult>(
            cacheKey: $"historical:{symbol}:{period1}:{period2}:{interval}",
            cacheTtl: TimeSpan.FromHours(6),
            uri: YahooEndpointBuilder.BuildHistoricalUri(symbol, period1, period2, interval.ToApiString()),
            ct
        );
    }

    /// <summary>
    /// Retrieves market insights and analysis for a given symbol.
    /// </summary>
    /// <remarks>
    /// Includes analyst sentiment, company outlook, and related insights.
    /// Cached for 12 hours.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing insight data.
    /// </returns>
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

    /// <summary>
    /// Retrieves the latest options chain for a symbol.
    /// </summary>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the options chain.
    /// </returns>
    public Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        CancellationToken ct = default)
        => GetOptionsAsync(symbol, null, ct);

    /// <summary>
    /// Retrieves the options chain for a symbol at a specific expiration date.
    /// </summary>
    /// <remarks>
    /// Cached for 15 minutes.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="date">Expiration date (Unix timestamp) or null for latest.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing options contracts.
    /// </returns>
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

    /// <summary>
    /// Retrieves real-time quote data for one or more symbols.
    /// </summary>
    /// <remarks>
    /// Cached briefly (30 seconds) due to high volatility.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="symbols">One or more ticker symbols.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing quote data.
    /// </returns>
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

    /// <summary>
    /// Retrieves detailed quote summary modules for a symbol.
    /// </summary>
    /// <remarks>
    /// Modules may include assetProfile, financialData, keyStatistics, etc.
    /// Cached for 2 hours.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="modules">Requested summary modules.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing quote summary data.
    /// </returns>
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

    /// <summary>
    /// Retrieves analyst recommendations for a symbol.
    /// </summary>
    /// <remarks>
    /// Includes buy/hold/sell ratings.
    /// Cached for 6 hours.
    /// </remarks>
    /// <param name="symbol">Ticker symbol.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing recommendation data.
    /// </returns>
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

    /// <summary>
    /// Executes a predefined Yahoo Finance screener.
    /// </summary>
    /// <remarks>
    /// Cached for 5 minutes.
    /// </remarks>
    /// <param name="screenerId">Identifier of the screener.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing screener results.
    /// </returns>
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

    /// <summary>
    /// Searches Yahoo Finance for symbols, companies, or instruments.
    /// </summary>
    /// <remarks>
    /// Cached for 10 minutes.
    /// </remarks>
    /// <param name="query">Search query text.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing search matches.
    /// </returns>
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

    /// <summary>
    /// Retrieves currently trending symbols for a given region.
    /// </summary>
    /// <remarks>
    /// Cached for 10 minutes.
    /// </remarks>
    /// <param name="region">Region code (e.g. US, GB).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing trending symbols.
    /// </returns>
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

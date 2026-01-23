using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Repository.YahooFinance;

public class YahooFinanceService : IYahooFinanceService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<YahooFinanceService> _logger;
    private readonly SemaphoreSlim _throttler;
    private const int MaxRequestsPerMinute = 50;

    public YahooFinanceService(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<YahooFinanceService> logger
    )
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _throttler = new SemaphoreSlim(MaxRequestsPerMinute);
    }

    public async Task<Result<ChartResultArray>> GetChartAsync(string symbol, YahooRange range, YahooInterval interval,
        CancellationToken cancellationToken = default)
    {
        var rangeStr = range.ToApiString();
        var intervalStr = interval.ToApiString();

        var cacheKey = $"chart:{symbol}:{rangeStr}:{intervalStr}";

        if (_cache.TryGetValue(cacheKey, out ChartResultArray? cachedResult) && cachedResult is not null)
        {
            return cachedResult;
        }

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            // Yahoo Finance Chart API v8
            // Example: https://query1.finance.yahoo.com/v8/finance/chart/AAPL?range=1d&interval=1m
            var url = YahooEndpointBuilder.Build(YahooEndpoint.Chart, symbol, range, interval);

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo Finance API Error: {response.StatusCode}");

            var result =
                await response.Content.ReadFromJsonAsync<ChartResultArray>(cancellationToken: cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize Yahoo Finance response");

            // Cache for 1 minute (charts update frequently during market hours)
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

            return Result<ChartResultArray>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetChartAsync for {Symbol}", symbol);
            return Result<ChartResultArray>.Fail(new InternalServerFailure500(ex.Message));
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<FundamentalsTimeSeriesResults>> GetFundamentalsTimeSeriesAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] types)
    {
        var typesStr = string.Join(",", types);
        var cacheKey = $"fundamentals:{symbol}:{typesStr}";

        if (_cache.TryGetValue(cacheKey, out FundamentalsTimeSeriesResults? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(
                YahooEndpoint.FundamentalsTimeSeries,
                symbol,
                types: types
            );

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<FundamentalsTimeSeriesResults>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize fundamentals response");

            _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFundamentalsTimeSeriesAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<HistoricalHistoryResult>> GetHistoricalAsync(
        string symbol,
        string period1,
        string period2,
        string interval,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"historical:{symbol}:{period1}:{period2}:{interval}";

        if (_cache.TryGetValue(cacheKey, out HistoricalHistoryResult? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(
                YahooEndpoint.Historical,
                symbol,
                period1: period1,
                period2: period2,
                interval: interval
            );

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<HistoricalHistoryResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize historical data");

            _cache.Set(cacheKey, result, TimeSpan.FromHours(6));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetHistoricalAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<InsightsResult>> GetInsightsAsync(string symbol,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"insights:{symbol}";

        if (_cache.TryGetValue(cacheKey, out InsightsResult? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.Insights, symbol);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<InsightsResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize insights");

            _cache.Set(cacheKey, result, TimeSpan.FromHours(12));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetInsightsAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public Task<Result<OptionsResult>> GetOptionsAsync(string symbol, CancellationToken cancellationToken = default)
        => GetOptionsAsync(symbol, null, cancellationToken);

    public async Task<Result<OptionsResult>> GetOptionsAsync(
        string symbol,
        string? date,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"options:{symbol}:{date ?? "latest"}";

        if (_cache.TryGetValue(cacheKey, out OptionsResult? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(
                YahooEndpoint.Options,
                symbol,
                date: date
            );

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<OptionsResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize options");

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(15));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetOptionsAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<QuoteResponseArray>> GetQuoteAsync(
        CancellationToken cancellationToken = default,
        params string[] symbols)
    {
        var symbolsStr = string.Join(",", symbols);
        var cacheKey = $"quote:{symbolsStr}";

        if (_cache.TryGetValue(cacheKey, out QuoteResponseArray? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.Quote, symbols: symbols);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<QuoteResponseArray>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize quotes");

            _cache.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetQuoteAsync failed");
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<QuoteSummaryResult>> GetQuoteSummaryAsync(
        string symbol,
        CancellationToken cancellationToken = default,
        params string[] modules)
    {
        var modulesStr = string.Join(",", modules);
        var cacheKey = $"quotesummary:{symbol}:{modulesStr}";

        if (_cache.TryGetValue(cacheKey, out QuoteSummaryResult? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(
                YahooEndpoint.QuoteSummary,
                symbol,
                modules: modules
            );

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<QuoteSummaryResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize quote summary");

            _cache.Set(cacheKey, result, TimeSpan.FromHours(2));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetQuoteSummaryAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<RecommendationsBySymbolResponseArray>> GetRecommendationsBySymbolAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.RecommendationsBySymbol, symbol);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result =
                await response.Content.ReadFromJsonAsync<RecommendationsBySymbolResponseArray>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize recommendations");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetRecommendationsBySymbolAsync failed for {Symbol}", symbol);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<ScreenerResult>> RunScreenerAsync(string screenerId,
        CancellationToken cancellationToken = default)
    {
        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.Screener, screenerId: screenerId);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<ScreenerResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize screener result");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunScreenerAsync failed for {ScreenerId}", screenerId);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<SearchResult>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.Search, query: query);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<SearchResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize search result");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchAsync failed for query {Query}", query);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }

    public async Task<Result<TrendingSymbolsResult>> GetTrendingSymbolsAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"trending:{region}";

        if (_cache.TryGetValue(cacheKey, out TrendingSymbolsResult? cached))
            return cached!;

        await _throttler.WaitAsync(cancellationToken);

        try
        {
            var url = YahooEndpointBuilder.Build(YahooEndpoint.TrendingSymbols, region: region);

            var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new InternalServerFailure500($"Yahoo API error: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<TrendingSymbolsResult>(cancellationToken);
            if (result is null)
                return new InternalServerFailure500("Failed to deserialize trending symbols");

            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetTrendingSymbolsAsync failed for region {Region}", region);
            return new InternalServerFailure500(ex.Message);
        }
        finally
        {
            _throttler.Release();
        }
    }
}
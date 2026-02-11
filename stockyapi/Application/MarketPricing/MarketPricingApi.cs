using System.Net;
using stockyapi.Application.MarketPricing.GetCurrentPrice;
using stockyapi.Middleware;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Application.MarketPricing;

public class MarketPricingApi : IMarketPricingApi
{
    private readonly IYahooFinanceService _yahooFinanceService;

    public MarketPricingApi(IYahooFinanceService yahooFinanceService)
    {
        _yahooFinanceService = yahooFinanceService;
    }

    public async Task<Result<ChartResultArray>> GetCurrentPrice(string ticker, CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.GetChartAsync(ticker, YahooRange.OneDay, YahooInterval.FifteenMinutes, [], cancellationToken);
    }

    public async Task<Result<ChartResultArray>> GetChart(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        YahooFields[]? additionalFields,
        CancellationToken cancellationToken)
    {
        var fields = new YahooFields[] { YahooFields.regularMarketPrice }.Concat(additionalFields ?? []).ToArray();
        return await _yahooFinanceService.GetChartAsync(symbol, range, interval, fields, cancellationToken);
    }

    public async Task<Result<HistoricalHistoryResult>> GetHistorical(
        string symbol,
        DateTime period1,
        DateTime period2,
        YahooInterval interval,
        CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.GetHistoricalAsync(symbol, period1, period2, interval, cancellationToken);
    }

    [Obsolete]
    public async Task<Result<QuoteResponseArray>> GetQuote(
        string[] symbols,
        CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.GetQuoteAsync(cancellationToken, symbols);
    }

    [Obsolete]
    public async Task<Result<QuoteSummaryResult>> GetQuoteSummary(
        string symbol,
        string[]? modules,
        CancellationToken cancellationToken)
    {
        modules ??= new[] { "price" };
        return await _yahooFinanceService.GetQuoteSummaryAsync(symbol, cancellationToken, modules);
    }

    public async Task<Result<ScreenerResult>> RunScreener(
        string screenerId,
        CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.RunScreenerAsync(screenerId, cancellationToken);
    }

    public async Task<Result<SearchResult>> Search(
        string query,
        CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.SearchAsync(query, cancellationToken);
    }

    public async Task<Result<TrendingSymbolsResult>> GetTrendingSymbols(
        string region,
        CancellationToken cancellationToken)
    {
        return await _yahooFinanceService.GetTrendingSymbolsAsync(region, cancellationToken);
    }
}
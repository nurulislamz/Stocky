using stockyapi.Middleware;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Application.MarketPricing;

public interface IMarketPricingApi
{
    Task<Result<ChartResultArray>> GetCurrentPrice(string ticker, CancellationToken cancellationToken);

    Task<Result<ChartResultArray>> GetChart(
        string symbol,
        YahooRange range,
        YahooInterval interval,
        YahooFields[]? additionalFields,
        CancellationToken cancellationToken);

    Task<Result<HistoricalHistoryResult>> GetHistorical(
        string symbol,
        DateTime period1,
        DateTime period2,
        YahooInterval interval,
        CancellationToken cancellationToken);

    Task<Result<QuoteResponseArray>> GetQuote(
        string[] symbols,
        CancellationToken cancellationToken);

    Task<Result<QuoteSummaryResult>> GetQuoteSummary(
        string symbol,
        string[]? modules,
        CancellationToken cancellationToken);

    Task<Result<ScreenerResult>> RunScreener(
        string screenerId,
        CancellationToken cancellationToken);

    Task<Result<SearchResult>> Search(
        string query,
        CancellationToken cancellationToken);

    Task<Result<TrendingSymbolsResult>> GetTrendingSymbols(
        string region,
        CancellationToken cancellationToken);
}

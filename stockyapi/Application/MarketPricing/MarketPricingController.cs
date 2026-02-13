using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.MarketPricing;
using stockyapi.Application.MarketPricing.GetCurrentPrice;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance.EndpointBuilder;
using stockyapi.Services.YahooFinance.Types;

namespace stockyapi.Controllers;

/// <summary>
/// Controller for retrieving market pricing data, charts, and financial information via Yahoo Finance.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class MarketPricingController : BaseController
{
    private readonly IMarketPricingApi _marketPricingApi;

    public MarketPricingController(
        IMarketPricingApi marketPricingApi)
    {
        _marketPricingApi = marketPricingApi;
    }

    /// <summary>
    /// Retrieves the current price and basic chart data for a specific ticker.
    /// </summary>
    /// <param name="ticker">The symbol to retrieve price data for (e.g., AAPL).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Chart result array containing price data.</returns>
    [HttpGet("{ticker}")]
    public async Task<ActionResult<YahooChartResponse>> GetCurrentPrice([FromRoute] string ticker, CancellationToken cancellationToken)
    {
        var response = await _marketPricingApi.GetCurrentPrice(ticker, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Retrieves chart data for a symbol with customizable range, interval, and additional fields.
    /// </summary>
    /// <param name="symbol">The stock symbol (e.g., MSFT).</param>
    /// <param name="range">The time range for the chart (default: 1 Day).</param>
    /// <param name="interval">The data interval (default: 1 Minute).</param>
    /// <param name="additionalFields">Optional additional fields to include in the response.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Chart data based on the specified parameters.</returns>
    [HttpGet("chart")]
    public async Task<IActionResult> GetChart(
        [FromQuery] string symbol,
        [FromQuery] YahooRange range = YahooRange.OneDay,
        [FromQuery] YahooInterval interval = YahooInterval.OneMinute,
        [FromQuery] YahooFields[]? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.GetChart(symbol, range, interval, additionalFields, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Retrieves historical price data for a symbol within a specified date range.
    /// </summary>
    /// <param name="symbol">The stock symbol.</param>
    /// <param name="period1">The start date of the period.</param>
    /// <param name="period2">The end date of the period.</param>
    /// <param name="interval">The data interval.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Historical data result.</returns>
    [HttpGet("historical")]
    public async Task<IActionResult> GetHistorical(
        [FromQuery] string symbol,
        [FromQuery] DateTime period1,
        [FromQuery] DateTime period2,
        [FromQuery] YahooInterval interval,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.GetHistorical(symbol, period1, period2, interval, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// DEPRECATED
    /// Retrieves real-time quotes for multiple symbols.
    /// </summary>
    /// <param name="symbols">Array of symbols to retrieve quotes for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote response array.</returns>
    [Obsolete]
    [HttpGet("quote")]
    public async Task<IActionResult> GetQuotes(
        [FromQuery] string[] symbols,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.GetQuote(symbols, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// DEPRECATED
    /// Retrieves a summary of financial data for a specific symbol, optionally filtering by modules.
    /// </summary>
    /// <param name="symbol">The stock symbol.</param>
    /// <param name="modules">Optional list of modules to retrieve (e.g., "price", "summaryDetail").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Quote summary result.</returns>
    [Obsolete]
    [HttpGet("quote-summary")]
    public async Task<IActionResult> GetQuoteSummary(
        [FromQuery] string symbol,
        [FromQuery] string[]? modules,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.GetQuoteSummary(symbol, modules, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Executes a predefined screener to find stocks matching specific criteria.
    /// </summary>
    /// <param name="screenerId">The ID of the predefined screener.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Screener result.</returns>
    [HttpGet("screener")]
    public async Task<IActionResult> RunScreener(
        [FromQuery] string screenerId,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.RunScreener(screenerId, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Searches for financial symbols or companies based on a query string.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search result containing matching symbols.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.Search(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    /// <summary>
    /// Retrieves trending symbols for a specific region.
    /// </summary>
    /// <param name="region">The region code (default: "US").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Trending symbols result.</returns>
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingSymbols(
        [FromQuery] string region = "US",
        CancellationToken cancellationToken = default)
    {
        var response = await _marketPricingApi.GetTrendingSymbols(region, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }
}
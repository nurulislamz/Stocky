using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance;

namespace stockyapi.Controllers;

/// <summary>
/// Controller for managing user funds.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HoldingDetailsController : BaseController
{
    private readonly ILogger<HoldingDetailsController> _logger;
    private readonly IYahooFinanceService _yahooFinanceService;

    public HoldingDetailsController(
        ILogger<HoldingDetailsController> logger,
        IYahooFinanceService yahooFinanceService
        )
    {
        _logger = logger;
        _yahooFinanceService = yahooFinanceService;
    }

    [HttpGet("fundamentals")]
    public async Task<IActionResult> GetFundamentalsTimeSeries(
        [FromQuery] string symbol,
        [FromQuery] string[]? types,
        CancellationToken cancellationToken = default)
    {
        types ??= new[] { "annual" };
        var response = await _yahooFinanceService.GetFundamentalsTimeSeriesAsync(symbol, cancellationToken, types);

        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    [HttpGet("insights")]
    public async Task<IActionResult> GetInsights(
        [FromQuery] string symbol,
        CancellationToken cancellationToken = default)
    {
        var response = await _yahooFinanceService.GetInsightsAsync(symbol, cancellationToken);

        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    [HttpGet("options")]
    public async Task<IActionResult> GetOptions(
        [FromQuery] string symbol,
        [FromQuery] string? date,
        CancellationToken cancellationToken = default)
    {
        var response = string.IsNullOrEmpty(date)
            ? await _yahooFinanceService.GetOptionsAsync(symbol, cancellationToken)
            : await _yahooFinanceService.GetOptionsAsync(symbol, date, cancellationToken);

        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetRecommendations(
        [FromQuery] string symbol,
        CancellationToken cancellationToken = default)
    {
        var response = await _yahooFinanceService.GetRecommendationsBySymbolAsync(symbol, cancellationToken);

        return response.IsSuccess ? Ok(response.Value) : ProcessFailure(response.Failure);
    }
}
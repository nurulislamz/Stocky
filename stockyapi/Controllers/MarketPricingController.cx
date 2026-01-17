using stockyapi.Requests;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.Portfolio;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketPricingController : ControllerBase
{
  private readonly ILogger<MarketPricingController> _logger;
  private readonly IMarketDataApi _marketDataApi;

  public MarketPricingController(
    ILogger<MarketPricingController> logger,
    IMarketDataApi marketDataApi)
  {
    _logger = logger;
    _marketDataApi = marketDataApi;
  }

  [HttpGet("current_data")]
  public async Task<ActionResult<CurrentPriceDataRequest>> GetMarketData([FromQuery] CurrentPriceDataRequest request,
    CancellationToken cancellationToken)
  {
    var response = await _marketDataApi.GetCurrentData(request, cancellationToken);
    return Ok(response);
  }

  [HttpGet("historical_prices")]
  public async Task<ActionResult<HistoricalPriceDataResponse>> GetHistoricalData([FromQuery] HistoricalPriceDataRequest request,
    CancellationToken cancellationToken)
  {
    var response = _marketDataApi.GetHistoricalData(request, cancellationToken);
    return Ok(response);
  }
  
  
  // General controller for getting historical current and other data about a stock
}
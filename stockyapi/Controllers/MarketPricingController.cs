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
}
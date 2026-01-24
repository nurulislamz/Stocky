using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Controllers.Helpers;

namespace stockyapi.Controllers;

/// <summary>
/// Controller for managing user funds.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HoldingDetailsController : BaseController
{
    private readonly ILogger<PortfolioController> _logger;

    public HoldingDetailsController(
        ILogger<PortfolioController> logger
        )
    {
        _logger = logger;
    }
    
    // TODO: Holds details about the holdings
    // Profit loss
    
    
    
    
}
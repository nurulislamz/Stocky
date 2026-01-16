using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stockyapi.Application.Portfolio;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PortfolioController : BaseController
{
    private readonly IPortfolioApi _portfolioApi;

    public PortfolioController(
        IPortfolioApi portfolioApi)
    {
        _portfolioApi = portfolioApi;
    }
    
    [HttpGet("holdings")]
    public async Task<ActionResult<ListHoldingsResponse>> ListHoldings(CancellationToken cancellationToken)
    {
        var response = await _portfolioApi.ListHoldings(cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpGet("holdings/id/{id}")]
    public async Task<ActionResult<GetHoldingsResponse>> GetHoldingsById(
        [FromRoute] 
        [RegularExpression(@"^[^,]+(,[^,]+)*$", ErrorMessage = "The request must be a comma-separated list with at least one item.")]
        [CommaSeparated]
        string[] ids, 
        CancellationToken cancellationToken)
    {
        var requestedHoldingIds = ids.Select(Guid.Parse);
        var response = await _portfolioApi.GetHoldingsById(requestedHoldingIds.ToArray(), cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpGet("holdings/ticker/{symbols}")]
    public async Task<ActionResult<GetHoldingsResponse>> GetHoldingsByTicker(
        [FromRoute]
        [RegularExpression(@"^[^,]+(,[^,]+)*$", ErrorMessage = "The request must be a comma-separated list with at least one item.")]
        [CommaSeparated]
        string[] symbols, 
        CancellationToken cancellationToken)
    {
        var response = await _portfolioApi.GetHoldingsByTicker(symbols, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpPost("buy")]
    public async Task<ActionResult<BuyTickerResponse>> Buy([FromBody] BuyTickerRequest request, CancellationToken cancellationToken)
    {
        var response= await _portfolioApi.BuyTicker(request, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }

    [HttpPost("sell")]
    public async Task<ActionResult<SellTickerResponse>> Sell([FromBody] SellTickerRequest request, CancellationToken cancellationToken)
    {
        var response= await _portfolioApi.SellTicker(request, cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpDelete("holdings/id/{id}")]
    public async Task<ActionResult<DeleteHoldingsResponse>> DeleteHoldingsById(
        [FromRoute] 
        [RegularExpression(@"^[^,]+(,[^,]+)*$", ErrorMessage = "The request must be a comma-separated list with at least one item.")]
        [CommaSeparated]
        string[] ids, 
        CancellationToken cancellationToken)
    {
        var requestedHoldingIds = ids.Select(Guid.Parse);
        var response = await _portfolioApi.DeleteHoldingsById(requestedHoldingIds.ToArray(), cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpDelete("holdings/ticker/{symbol}")]
    public async Task<ActionResult<DeleteHoldingsResponse>> DeleteHoldingsByTicker(
        [FromRoute]
        [RegularExpression(@"^[^,]+(,[^,]+)*$", ErrorMessage = "The request must be a comma-separated list with at least one item.")]
        [CommaSeparated]
        string[] symbols, 
        CancellationToken cancellationToken)
    {
        var requestedHoldingIds = symbols.Select(Guid.Parse);
        var response = await _portfolioApi.DeleteHoldingsById(requestedHoldingIds.ToArray(), cancellationToken);
        return response.IsSuccess ? ProcessSuccess(HttpStatusCode.OK, response.Value) : ProcessFailure(response.Failure);
    }
    
    [HttpPut("holdings/ticker/{symbol}")]
    public async Task<ActionResult<DeleteHoldingsResponse>> UpdateHoldingsByTicker(
        [FromRoute]
        string symbol, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

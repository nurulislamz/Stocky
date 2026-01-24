using stockyapi.Application.Portfolio.BuyTicker;
using stockyapi.Application.Portfolio.DeleteHolding;
using stockyapi.Application.Portfolio.GetHoldings;
using stockyapi.Application.Portfolio.ListHoldings;
using stockyapi.Application.Portfolio.SellTicker;
using stockyapi.Middleware;

namespace stockyapi.Application.Portfolio;

public interface IPortfolioApi
{
    Task<Result<ListHoldingsResponse>> ListHoldings(CancellationToken cancellationToken);
    Task<Result<GetHoldingsResponse>> GetHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken);
    Task<Result<GetHoldingsResponse>> GetHoldingsByTicker(string[] requestedTickers, CancellationToken cancellationToken);
    Task<Result<BuyTickerResponse>> BuyTicker(BuyTickerRequest request, CancellationToken cancellationToken);
    Task<Result<SellTickerResponse>> SellTicker(SellTickerRequest request, CancellationToken cancellationToken);
    Task<Result<DeleteHoldingsResponse>> DeleteHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken);
    Task<Result<DeleteHoldingsResponse>> DeleteHoldingsByTicker(string[] requestedTickers, CancellationToken cancellationToken);
    Task<Result<GetHoldingsResponse>> UpdateHoldings(string request, CancellationToken cancellationToken);
}
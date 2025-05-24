using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class BuyTickerInPortfolioRequestHandler : IRequestHandler<BuyTickerRequest, BuyTickerResponse>
{
    private readonly IPortfolioService _portfolioService;

    public BuyTickerInPortfolioRequestHandler(IPortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public async Task<BuyTickerResponse> Handle(BuyTickerRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.BuyTickerInPortfolio(request);
    }
}
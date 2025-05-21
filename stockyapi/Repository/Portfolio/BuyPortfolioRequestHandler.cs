using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class BuyPortfolioRequestHandler : IRequestHandler<BuyTickerRequest, BuyTickerResponse>
{
    private readonly PortfolioService _portfolioService;

    public BuyPortfolioRequestHandler(PortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public Task<BuyTickerResponse> Handle(BuyTickerRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
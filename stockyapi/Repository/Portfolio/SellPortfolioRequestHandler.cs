using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class SellPortfolioRequestHandler : IRequestHandler<SellTickerRequest, SellTickerResponse>
{
    private readonly PortfolioService _portfolioService;

    public SellPortfolioRequestHandler(PortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public Task<SellTickerResponse> Handle(SellTickerRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
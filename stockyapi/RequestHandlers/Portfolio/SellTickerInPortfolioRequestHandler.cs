using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class SellTickerInPortfolioRequestHandler : IRequestHandler<SellTickerRequest, SellTickerResponse>
{
    private readonly IPortfolioService _portfolioService;

    public SellTickerInPortfolioRequestHandler(IPortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public async Task<SellTickerResponse> Handle(SellTickerRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.SellTickerInPortfolio(request);
    }
}
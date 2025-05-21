using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class GetUserPortfolioRequestHandler : IRequestHandler<GetPortfolioRequest, GetPortfolioResponse>
{
    private readonly PortfolioService _portfolioService;

    public GetUserPortfolioRequestHandler(PortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public async Task<GetPortfolioResponse> Handle(GetPortfolioRequest request, CancellationToken cancellationToken)
    {
        var portfolio =  await _portfolioService.FetchUserPortfolio();
        return _portfolioService.CreatePortfolioResponse(portfolio);
    }
}
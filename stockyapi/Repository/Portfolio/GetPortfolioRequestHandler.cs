using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Portfolio;

public class GetPortfolioRequestHandler : IRequestHandler<GetPortfolioRequest, GetPortfolioResponse>
{
    private readonly PortfolioService _portfolioService;

    public GetPortfolioRequestHandler(PortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public async Task<GetPortfolioResponse> Handle(GetPortfolioRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.GetUserPortfolio();
    }
}
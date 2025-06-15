using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;

namespace stockyapi.RequestHandlers;

public class GetUserPortfolioRequestHandler : IRequestHandler<UserPortfolioRequest, UserPortfolioResponse>
{
    private readonly IPortfolioService _portfolioService;

    public GetUserPortfolioRequestHandler(IPortfolioService portfolioservice)
    {
        _portfolioService = portfolioservice;
    }

    public async Task<UserPortfolioResponse> Handle(UserPortfolioRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.GetUserPortfolio();
    }
}
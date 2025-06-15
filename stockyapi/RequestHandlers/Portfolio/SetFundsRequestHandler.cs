using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;

namespace stockyapi.RequestHandlers;

public class SetFundsRequestHandler : IRequestHandler<SetFundsRequest, SetFundsResponse>
{
    private readonly IPortfolioService _portfolioService;

    public SetFundsRequestHandler(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public async Task<SetFundsResponse> Handle(SetFundsRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.SetFunds(request);
    }
}
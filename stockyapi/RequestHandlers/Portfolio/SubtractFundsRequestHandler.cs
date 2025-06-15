using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;

namespace stockyapi.RequestHandlers;

public class SubtractFundsRequestHandler : IRequestHandler<SubtractFundsRequest, SubtractFundsResponse>
{
    private readonly IPortfolioService _portfolioService;

    public SubtractFundsRequestHandler(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public async Task<SubtractFundsResponse> Handle(SubtractFundsRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.SubtractFunds(request);
    }
}
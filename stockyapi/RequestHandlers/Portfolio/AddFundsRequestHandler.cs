using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;

namespace stockyapi.RequestHandlers;

public class AddFundsRequestHandler : IRequestHandler<AddFundsRequest, AddFundsResponse>
{
    private readonly IPortfolioService _portfolioService;

    public AddFundsRequestHandler(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public async Task<AddFundsResponse> Handle(AddFundsRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.AddFunds(request);
    }
}
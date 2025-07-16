using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;

namespace stockyapi.RequestHandlers;

public class DeleteTickerInPortfolioRequestHandler : IRequestHandler<DeleteTickerRequest, DeleteTickerResponse>
{
    private readonly IPortfolioService _portfolioService;

    public DeleteTickerInPortfolioRequestHandler(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public async Task<DeleteTickerResponse> Handle(DeleteTickerRequest request, CancellationToken cancellationToken)
    {
        return await _portfolioService.DeleteTicker(request);
    }
}
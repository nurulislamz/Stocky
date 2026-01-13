using stockyapi.Middleware;
using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Application.Portfolio;

public interface IFundsApi
{
    public Task<Result<FundsResponse>> GetFunds(CancellationToken cancellationToken);
    public Task<Result<FundsResponse>> DepositFunds(DepositFundsRequest request, CancellationToken cancellationToken);
    public Task<Result<FundsResponse>> WithdrawFunds(WithdrawFundsRequest request, CancellationToken cancellationToken);
}
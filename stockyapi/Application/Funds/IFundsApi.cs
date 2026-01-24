using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.Response;
using stockyapi.Application.Funds.SubtractFunds;
using stockyapi.Middleware;

namespace stockyapi.Application.Funds;

public interface IFundsApi
{
    public Task<Result<FundsResponse>> GetFunds(CancellationToken cancellationToken);
    public Task<Result<FundsResponse>> DepositFunds(DepositFundsRequest request, CancellationToken cancellationToken);
    public Task<Result<FundsResponse>> WithdrawFunds(WithdrawFundsRequest request, CancellationToken cancellationToken);
}
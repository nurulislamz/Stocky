using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Application.Portfolio;

public interface IMarketPricingApi
{
    public Task<FundsResponse> GetCurrentData(DepositFundsRequest request, CancellationToken cancellationToken);
    public Task<FundsResponse> GetHistoricalData(WithdrawFundsRequest request, CancellationToken cancellationToken);
}
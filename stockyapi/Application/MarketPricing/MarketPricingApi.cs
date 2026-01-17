using stockyapi.Requests;
using stockyapi.Responses;

namespace stockyapi.Application.Portfolio;

public interface IMarketDataApi
{
    public Task<FundsResponse> GetCurrentData(DepositFundsRequest request, CancellationToken cancellationToken);
    public Task GetHistoricalData(WithdrawFundsRequest request, CancellationToken cancellationToken);
}
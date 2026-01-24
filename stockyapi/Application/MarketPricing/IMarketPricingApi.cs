using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.Response;
using stockyapi.Application.Funds.SubtractFunds;

namespace stockyapi.Application.MarketPricing;

public interface IMarketPricingApi
{
    public Task<FundsResponse> GetCurrentData(DepositFundsRequest request, CancellationToken cancellationToken);
    public Task<FundsResponse> GetHistoricalData(WithdrawFundsRequest request, CancellationToken cancellationToken);
}
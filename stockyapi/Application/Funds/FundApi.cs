using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.Response;
using stockyapi.Application.Funds.SubtractFunds;
using stockyapi.Middleware;
using stockyapi.Repository.Funds;
using stockyapi.Repository.PortfolioRepository;
using stockymodels.Models.Enums;

namespace stockyapi.Application.Funds;

public sealed class FundsApi : IFundsApi
{
    private readonly IUserContext _userContext;
    private readonly IFundsRepository _fundsRepository;
    
    public FundsApi(IUserContext userContext, IFundsRepository fundsRepository)
    {
        _userContext = userContext;
        _fundsRepository = fundsRepository;
    }
    
    public async Task<Result<FundsResponse>> GetFunds(CancellationToken cancellationToken)
    {
        var fundResponse = await _fundsRepository.GetFundsAsync(_userContext.UserId, cancellationToken);
        
        return Result<FundsResponse>.Success(new FundsResponse(fundResponse.CashBalance, fundResponse.TotalValue, fundResponse.InvestedAmount));
    }

    public async Task<Result<FundsResponse>> DepositFunds(DepositFundsRequest request, CancellationToken cancellationToken)
    {
        var updateFunds = await _fundsRepository.DepositFundsAsync(_userContext.UserId, request.Amount, cancellationToken);
        return new FundsResponse(updateFunds.CashBalance, updateFunds.TotalValue, updateFunds.InvestedAmount);
    }
    
    public async Task<Result<FundsResponse>> WithdrawFunds(WithdrawFundsRequest request, CancellationToken cancellationToken)
    {
        var userFunds =  await _fundsRepository.GetFundsAsync(_userContext.UserId, cancellationToken);
        if (userFunds.CashBalance < request.Amount)
            return new BadRequestFailure400($"Insufficient funds. Withdraw amount {request.Amount} is greater than user balance {request.Amount}");
        
        var updateFunds = await _fundsRepository.WithdrawFundsAsync(_userContext.UserId, request.Amount, cancellationToken);
        return new FundsResponse(updateFunds.CashBalance, updateFunds.TotalValue, updateFunds.InvestedAmount);
    }
}

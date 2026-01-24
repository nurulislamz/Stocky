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
    private readonly IPortfolioRepository _portfolioRepository;
    
    public FundsApi(IUserContext userContext, IFundsRepository fundsRepository, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _fundsRepository = fundsRepository;
        _portfolioRepository = portfolioRepository;
    }
    
    public async Task<Result<FundsResponse>> GetFunds(CancellationToken cancellationToken)
    {
        var fundResponse = await _fundsRepository.GetFundsAsync(_userContext.UserId, cancellationToken);
        
        return Result<FundsResponse>.Success(new FundsResponse(fundResponse.CashBalance, fundResponse.TotalValue, fundResponse.InvestedAmount));
    }

    public async Task<Result<FundsResponse>> DepositFunds(DepositFundsRequest request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetPortfolioFromUserIdAsync(_userContext.UserId, cancellationToken);
        
        var cashDelta = request.Amount;
        
        var updateFunds = await _fundsRepository.DepositFundsAsync(portfolio.UserId, portfolio.Id, cashDelta, cancellationToken);
        return Result<FundsResponse>.Success(new FundsResponse(updateFunds.CashBalance, updateFunds.TotalValue, updateFunds.InvestedAmount));
    }
    
    public async Task<Result<FundsResponse>> WithdrawFunds(WithdrawFundsRequest request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetPortfolioFromUserIdAsync(_userContext.UserId, cancellationToken);
        
        var cashDelta = -request.Amount;
        
        var updateFunds = await _fundsRepository.WithdrawFundsAsync(portfolio.UserId, portfolio.Id, cashDelta, cancellationToken);
        return Result<FundsResponse>.Success(new FundsResponse(updateFunds.CashBalance, updateFunds.TotalValue, updateFunds.InvestedAmount));
    }
}

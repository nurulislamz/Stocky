using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.CommandAndQueries;
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
        if (fundResponse.IsFailure)
            return fundResponse.Failure;
        
        return Result<FundsResponse>.Success(new FundsResponse(fundResponse.Value.CashBalance, fundResponse.Value.TotalValue, fundResponse.Value.InvestedAmount));
    }

    public async Task<Result<FundsResponse>> DepositFunds(DepositFundsRequest request, CancellationToken cancellationToken)
    {
        return await ProcessTransaction(request.Amount, FundOperationType.Deposit, cancellationToken);
    }
    
    public async Task<Result<FundsResponse>> WithdrawFunds(WithdrawFundsRequest request, CancellationToken cancellationToken)
    {
        return await ProcessTransaction(request.Amount, FundOperationType.Withdrawal, cancellationToken);
    }

    private async Task<Result<FundsResponse>> ProcessTransaction(decimal amount, FundOperationType transactionType, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetPortfolioFromUserIdAsync(_userContext.UserId, cancellationToken);
        
        var cashDelta = transactionType == FundOperationType.Deposit ? amount : -amount;
        if (cashDelta < 0 && portfolio.CashBalance + cashDelta < 0)
            return new ConflictFailure409("Insufficient funds to withdraw.");
        
        // Add a record to the funds transaction table
        BaseFundCommand command = transactionType switch
        {
            FundOperationType.Deposit => new DepositFundCommands(portfolio.UserId, portfolio.Id, cashDelta),
            FundOperationType.Withdrawal => new WithdrawOrderCommand(portfolio.UserId, portfolio.Id, cashDelta),
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
        
        var updateFunds = await _fundsRepository.ApplyFundTransactionAsync(command, cancellationToken);
        if (updateFunds.IsFailure)
            return updateFunds.Failure;
        
        return Result<FundsResponse>.Success(new FundsResponse(updateFunds.Value.CashBalance, updateFunds.Value.TotalValue, updateFunds.Value.InvestedAmount));
    }
}

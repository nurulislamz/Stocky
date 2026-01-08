using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Portfolio;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Repository.Portfolio;

public class FundsRepository : IFundsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FundsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Funds repository
    public async Task<Result<PortfolioBalances>> GetFundsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var portfolio = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => new PortfolioBalances(p.CashBalance, p.InvestedAmount, p.InvestedAmount))
            .SingleOrDefaultAsync(cancellationToken);

        if (portfolio == null)
            return Result<PortfolioBalances>.Fail(new NotFoundFailure404("Portfolio not found"));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result<PortfolioBalances>.Success(portfolio);
    }

    public async Task<Result<PortfolioBalances>> ApplyFundTransactionAsync(BaseFundCommand command, CancellationToken ct)
    {
        var fundTransaction = CreateFundsTransactionModel(command.UserId, command.PortfolioId, command.CashDelta, command.OperationType);
        
        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.Id == fundTransaction.PortfolioId, ct);
        
        if (portfolio == null)
            return Result<PortfolioBalances>.Fail(new NotFoundFailure404("Portfolio not found"));
        
        var updatedCashBalance = portfolio.CashBalance + command.CashDelta;
        var updatedTotalValue = portfolio.TotalValue + command.CashDelta;

        portfolio.CashBalance = updatedCashBalance;
        portfolio.TotalValue = updatedTotalValue;

        await _dbContext.FundsTransactions.AddAsync(fundTransaction, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return Result<PortfolioBalances>.Success(new (portfolio.CashBalance, portfolio.TotalValue, portfolio.InvestedAmount));
    }

    private static FundsTransactionModel CreateFundsTransactionModel(Guid userId, Guid portfolioId, decimal cashDelta,
        FundOperationType operationType)
     => new () { Id = userId, PortfolioId = portfolioId, Type = operationType, CashAmount = Math.Abs(cashDelta) };
    
    
}
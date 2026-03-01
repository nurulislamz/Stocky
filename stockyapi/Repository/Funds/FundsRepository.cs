using Microsoft.EntityFrameworkCore;
using stockyapi.Repository.Funds.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.Funds;

public class FundsRepository : IFundsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FundsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Funds repository
    public async Task<PortfolioBalances> GetFundsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var portfolio = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => new PortfolioBalances(p.CashBalance, p.InvestedAmount, p.TotalValue))
            .SingleOrDefaultAsync(cancellationToken);

        // TODO: Fix all the exception handling code to also log and do other stuff.
        if (portfolio == null)
            throw new Exception("portfolio not found");

        return portfolio;
    }

    public async Task<PortfolioBalances> DepositFundsAsync(Guid userId, decimal cashDelta, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.UserId == userId, ct);

        if (portfolio == null)
            throw new Exception("portfolio not found.");

        var fundTransaction = CreateFundsTransactionModel(userId, portfolio.Id, cashDelta, FundOperationType.Deposit);

        var updatedCashBalance = portfolio.CashBalance + cashDelta;
        var updatedTotalValue = portfolio.TotalValue + cashDelta;

        if (portfolio.CashBalance > updatedCashBalance)
            throw new Exception($"Deposit somehow resulted in cashBalance decreasing??? PortfolioCashBalance: {portfolio.CashBalance}, UpdatedCashDelta: {updatedCashBalance}, CashDelta: {cashDelta}");

        if (portfolio.TotalValue > updatedTotalValue)
            throw new Exception($"Deposit somehow resulted in totalValue decreasing??? TotalValue: {portfolio.TotalValue}, UpdatedTotalValue: {updatedTotalValue}, CashDelta: {cashDelta}");

        portfolio.CashBalance = updatedCashBalance;
        portfolio.TotalValue = updatedTotalValue;

        await _dbContext.FundsTransactions.AddAsync(fundTransaction, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new PortfolioBalances(portfolio.CashBalance, portfolio.InvestedAmount, portfolio.TotalValue);
    }

    public async Task<PortfolioBalances> WithdrawFundsAsync(Guid userId, decimal cashDelta, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.UserId == userId, ct);

        if (portfolio == null)
            throw new Exception("portfolio not found.");

        var fundTransaction = CreateFundsTransactionModel(userId, portfolio.Id, cashDelta, FundOperationType.Withdrawal);

        var updatedCashBalance = portfolio.CashBalance - cashDelta;
        var updatedTotalValue = portfolio.TotalValue - cashDelta;

        if (portfolio.CashBalance < updatedCashBalance)
            throw new Exception($"Withdraw somehow resulted in cashBalance increasing??? PortfolioCashBalance: {portfolio.CashBalance}, UpdatedCashDelta: {updatedCashBalance}, CashDelta: {cashDelta}");

        if (portfolio.TotalValue < updatedTotalValue)
            throw new Exception($"Withdraw somehow resulted in totalValue increasing??? TotalValue: {portfolio.TotalValue}, UpdatedTotalValue: {updatedTotalValue}, CashDelta: {cashDelta}");

        portfolio.CashBalance = updatedCashBalance;
        portfolio.TotalValue = updatedTotalValue;

        await _dbContext.FundsTransactions.AddAsync(fundTransaction, ct);
        await _dbContext.SaveChangesAsync(ct);

        return new PortfolioBalances(portfolio.CashBalance, portfolio.InvestedAmount, portfolio.TotalValue);
    }

    private static FundsTransactionModel CreateFundsTransactionModel(Guid userId, Guid portfolioId, decimal cashDelta,
        FundOperationType operationType)
     => new () { Id = userId, PortfolioId = portfolioId, Type = operationType, CashAmount = Math.Abs(cashDelta) };


}
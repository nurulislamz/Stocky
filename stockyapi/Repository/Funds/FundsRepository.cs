using Microsoft.EntityFrameworkCore;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.Funds;

public class FundsRepository : IFundsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FundsRepository> _logger;

    public FundsRepository(ApplicationDbContext dbContext, ILogger<FundsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // Funds repository
    public async Task<PortfolioBalances> GetFundsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var portfolio = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => new PortfolioBalances(p.CashBalance, p.InvestedAmount, p.TotalValue))
            .SingleOrDefaultAsync(cancellationToken);

        if (portfolio == null)
        {
            var exception = new InvalidOperationException("Portfolio not found for user.");
            _logger.LogError(LoggingEventIds.FundsPortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        return portfolio;
    }

    public async Task<PortfolioBalances> DepositFundsAsync(Guid userId, decimal cashDelta, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.UserId == userId, ct);

        if (portfolio == null)
        {
            var exception = new InvalidOperationException("Portfolio not found for user.");
            _logger.LogError(LoggingEventIds.FundsPortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var fundTransaction = CreateFundsTransactionModel(userId, portfolio.Id, cashDelta, FundOperationType.Deposit);

        var updatedCashBalance = portfolio.CashBalance + cashDelta;
        var updatedTotalValue = portfolio.TotalValue + cashDelta;

        if (portfolio.CashBalance > updatedCashBalance)
        {
            var exception = new InvalidOperationException("Deposit resulted in cash balance decreasing.");
            _logger.LogError(LoggingEventIds.FundsDepositInvariantViolation, exception,
                "Deposit invariant violation: CashBalance {CashBalance}, UpdatedCashBalance {UpdatedCashBalance}, CashDelta {CashDelta}, UserId {UserId}",
                portfolio.CashBalance, updatedCashBalance, cashDelta, userId);
            throw exception;
        }

        if (portfolio.TotalValue > updatedTotalValue)
        {
            var exception = new InvalidOperationException("Deposit resulted in total value decreasing.");
            _logger.LogError(LoggingEventIds.FundsDepositInvariantViolation, exception,
                "Deposit invariant violation: TotalValue {TotalValue}, UpdatedTotalValue {UpdatedTotalValue}, CashDelta {CashDelta}, UserId {UserId}",
                portfolio.TotalValue, updatedTotalValue, cashDelta, userId);
            throw exception;
        }

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
        {
            var exception = new InvalidOperationException("Portfolio not found for user.");
            _logger.LogError(LoggingEventIds.FundsPortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var fundTransaction = CreateFundsTransactionModel(userId, portfolio.Id, cashDelta, FundOperationType.Withdrawal);

        var updatedCashBalance = portfolio.CashBalance - cashDelta;
        var updatedTotalValue = portfolio.TotalValue - cashDelta;

        if (portfolio.CashBalance < updatedCashBalance)
        {
            var exception = new InvalidOperationException("Withdraw resulted in cash balance increasing.");
            _logger.LogError(LoggingEventIds.FundsWithdrawInvariantViolation, exception,
                "Withdraw invariant violation: CashBalance {CashBalance}, UpdatedCashBalance {UpdatedCashBalance}, CashDelta {CashDelta}, UserId {UserId}",
                portfolio.CashBalance, updatedCashBalance, cashDelta, userId);
            throw exception;
        }

        if (portfolio.TotalValue < updatedTotalValue)
        {
            var exception = new InvalidOperationException("Withdraw resulted in total value increasing.");
            _logger.LogError(LoggingEventIds.FundsWithdrawInvariantViolation, exception,
                "Withdraw invariant violation: TotalValue {TotalValue}, UpdatedTotalValue {UpdatedTotalValue}, CashDelta {CashDelta}, UserId {UserId}",
                portfolio.TotalValue, updatedTotalValue, cashDelta, userId);
            throw exception;
        }

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
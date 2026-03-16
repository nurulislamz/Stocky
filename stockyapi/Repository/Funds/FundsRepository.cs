using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Commands.Funds;
using stockyapi.Middleware;
using stockyapi.Repository.Event;
using stockyapi.Repository.Funds.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.Funds;

public class FundsRepository : IFundsRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<FundsRepository> _logger;

    public FundsRepository(
        ApplicationDbContext dbContext,
        IEventRepository eventRepository,
        ILogger<FundsRepository> logger)
    {
        _dbContext = dbContext;
        _eventRepository = eventRepository;
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

    public async Task<PortfolioBalances> DepositFundsAsync(Guid userId, DepositFundsCommand command, CancellationToken ct)
    {
        var cashDelta = command.Amount;

        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.UserId == userId, ct);

        if (portfolio == null)
        {
            var exception = new InvalidOperationException("Portfolio not found for user.");
            _logger.LogError(LoggingEventIds.FundsPortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

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

        var maxSeq = await _dbContext.EventModels
            .Where(e => e.AggregateType == AggregateType.PortfolioId && e.AggregateId == portfolio.Id)
            .MaxAsync(e => (int?)e.SequenceId, ct);
        var nextSequenceId = (maxSeq ?? 0) + 1;

        var evt = _eventRepository.CreateEvent(
            userId,
            AggregateType.PortfolioId,
            portfolio.Id,
            nextSequenceId,
            EventType.FundsDepositedEvent,
            command,
            traceId: null,
            commandId: null,
            ct);

        portfolio.CashBalance = updatedCashBalance;
        portfolio.TotalValue = updatedTotalValue;

        _eventRepository.Add(evt);
        await _dbContext.SaveChangesAsync(ct);

        return new PortfolioBalances(portfolio.CashBalance, portfolio.InvestedAmount, portfolio.TotalValue);
    }

    public async Task<PortfolioBalances> WithdrawFundsAsync(Guid userId, WithdrawFundsCommand command, CancellationToken ct)
    {
        var cashDelta = command.Amount;

        var portfolio = await _dbContext.Portfolios
            .SingleOrDefaultAsync(p => p.UserId == userId, ct);

        if (portfolio == null)
        {
            var exception = new InvalidOperationException("Portfolio not found for user.");
            _logger.LogError(LoggingEventIds.FundsPortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

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

        var maxSeq = await _dbContext.EventModels
            .Where(e => e.AggregateType == AggregateType.PortfolioId && e.AggregateId == portfolio.Id)
            .MaxAsync(e => (int?)e.SequenceId, ct);
        var nextSequenceId = (maxSeq ?? 0) + 1;

        var evt = _eventRepository.CreateEvent(
            userId,
            AggregateType.PortfolioId,
            portfolio.Id,
            nextSequenceId,
            EventType.FundsWithdrawnEvent,
            command,
            traceId: null,
            commandId: null,
            ct);

        portfolio.CashBalance = updatedCashBalance;
        portfolio.TotalValue = updatedTotalValue;

        _eventRepository.Add(evt);
        await _dbContext.SaveChangesAsync(ct);

        return new PortfolioBalances(portfolio.CashBalance, portfolio.InvestedAmount, portfolio.TotalValue);
    }

}
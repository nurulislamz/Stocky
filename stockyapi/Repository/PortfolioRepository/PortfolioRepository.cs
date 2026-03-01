using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Commands.Portfolio;
using stockyapi.Middleware;
using stockyapi.Repository.Event;
using stockyapi.Repository.Funds.Types;
using stockyapi.Repository.PortfolioRepository.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.PortfolioRepository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<PortfolioRepository> _logger;

    public PortfolioRepository(
        ApplicationDbContext dbContext,
        IEventRepository eventRepository,
        ILogger<PortfolioRepository> logger)
    {
        _dbContext = dbContext;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<PortfolioModel> GetPortfolioFromUserIdAsync(Guid userId, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }
        return portfolio;
    }

    public async Task<PortfolioWithHoldings> ListAllHoldingsAsync(Guid userId, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .SingleOrDefaultAsync(ct);

        if (portfolio is null)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var allHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolio.Id).ToListAsync(ct);
        return new PortfolioWithHoldings(portfolio.CashBalance, portfolio.TotalValue, portfolio.InvestedAmount, allHoldings);
    }

    public async Task<HoldingsValidationResult<Guid>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedIds, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        return await ValidateHoldingsExist(portfolioId, requestedIds.ToHashSet(), ct);
    }

    public async Task<HoldingsValidationResult<string>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        return await ValidateHoldingsExist(portfolioId, requestedTickers.ToHashSet(), ct);
    }

    public async Task<TradeResult> BuyHoldingAsync(Guid userId, StockBoughtCommand command, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new InvalidOperationException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var totalCost = command.Quantity * command.Price;

        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;

        var existingHolding = await _dbContext.StockHoldings
            .SingleOrDefaultAsync(h => h.PortfolioId == portfolio.Id && h.Ticker == command.Symbol, ct);

        decimal newTotalShares = (existingHolding?.Shares ?? 0) + command.Quantity;
        decimal newTotalCost = ((existingHolding?.AverageCost ?? 0) * (existingHolding?.Shares ?? 0)) + totalCost;
        decimal newAverageCost = newTotalShares > 0 ? newTotalCost / newTotalShares : command.Price;

        if (existingHolding != null)
        {
            existingHolding.Shares = newTotalShares;
            existingHolding.AverageCost = newAverageCost;
        }
        else
        {
            _dbContext.StockHoldings.Add(new StockHoldingModel
            {
                PortfolioId = portfolio.Id,
                Ticker = command.Symbol,
                Shares = newTotalShares,
                AverageCost = newAverageCost,
            });
        }

        var eventId = Guid.NewGuid();
        var evt = await CreateEvent(AggregateType.PortfolioId, portfolio.Id, EventType.StockBought, command, ct);

        _eventRepository.Add(evt);
        await _dbContext.SaveChangesAsync(ct);

        return new TradeResult(evt.Id, command.Symbol, command.Quantity, command.Price, totalCost, newAverageCost, portfolio);
    }

    public async Task<TradeResult> SellHoldingAsync(Guid userId, StockSoldCommand command, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new InvalidOperationException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var totalProceeds = command.Quantity * command.Price;

        portfolio.CashBalance += totalProceeds;
        portfolio.InvestedAmount -= totalProceeds;

        var holding = await _dbContext.StockHoldings
            .SingleAsync(h => h.PortfolioId == portfolio.Id && h.Ticker == command.Symbol, ct);

        var newTotalShares = holding.Shares - command.Quantity;
        var newTotalCost = holding.AverageCost * holding.Shares;
        var newAverageCost = newTotalShares == 0 ? 0 : newTotalCost / newTotalShares;

        if (newTotalShares == 0)
        {
            _dbContext.StockHoldings.Remove(holding);
        }
        else
        {
            holding.Shares = newTotalShares;
            holding.AverageCost = newAverageCost;
        }

        var evt = await CreateEvent(AggregateType.PortfolioId, portfolio.Id, EventType.StockSold, command, ct);

        _eventRepository.Add(evt);
        await _dbContext.SaveChangesAsync(ct);

        return new TradeResult(evt.Id, command.Symbol, command.Quantity, command.Price, totalProceeds,newAverageCost, portfolio);
    }

    public async Task<List<Guid>> DeleteHoldingsAsync(Guid userId, List<StockHoldingModel> holdings, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new InvalidOperationException($"PortfolioId not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(LoggingEventIds.PortfolioNotFound, exception, "Portfolio not found for UserId {UserId}", userId);
            throw exception;
        }

        var tickers = holdings.Select(h => h.Ticker).ToArray();
        var command = new DeleteHoldingCommand(
            holdings.Select(h => h.Id).ToArray(),
            tickers);

        var evt = await CreateEvent(AggregateType.PortfolioId, portfolioId, EventType.DeleteHolding, command, ct);

        _dbContext.StockHoldings.RemoveRange(holdings);
        _eventRepository.Add(evt);
        await _dbContext.SaveChangesAsync(ct);

        return holdings.Select(h => h.Id).ToList();
    }

    private async Task<EventModel> CreateEvent<TCommand>(
        AggregateType aggregateType,
        Guid aggregateId,
        EventType eventType,
        TCommand command,
        CancellationToken ct)
    {
        var maxSeq = await _dbContext.EventModels
            .Where(e => e.AggregateType == aggregateType && e.AggregateId == aggregateId)
            .MaxAsync(e => (int?)e.SequenceId, ct);
        var nextSequenceId = (maxSeq ?? 0) + 1;

        var now = DateTimeOffset.UtcNow;
        var validTo = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        return new EventModel
        {
            Id = Guid.NewGuid(),
            AggregateType = aggregateType,
            AggregateId = aggregateId,
            SequenceId = nextSequenceId,
            EventType = eventType,
            EventPayloadJson = JsonSerializer.Serialize(command),
            EventPayloadProtobuf = Array.Empty<byte>(),
            TtStart = now,
            TtEnd = now,
            ValidFrom = now,
            ValidTo = validTo
        };
    }

    private async Task<HoldingsValidationResult<Guid>> ValidateHoldingsExist(Guid portfolioId, HashSet<Guid> requestedIds, CancellationToken ct)
    {
        var foundHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .Where(h => requestedIds.Contains(h.Id))
            .ToListAsync(ct);

        var foundHoldingIds = foundHoldings.Select(g => g.Id);
        var invalidIds = requestedIds.Where(s => !foundHoldingIds.Contains(s)).ToList();

        return new HoldingsValidationResult<Guid>(foundHoldings, invalidIds);
    }

    private async Task<HoldingsValidationResult<string>> ValidateHoldingsExist(Guid portfolioId, HashSet<string> requestedTickers, CancellationToken ct)
    {
        var foundHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .Where(h => requestedTickers.Contains(h.Ticker))
            .ToListAsync(ct);

        var foundTickers = foundHoldings.Select(g => g.Ticker);
        var invalidTickers = requestedTickers.Where(s => !foundTickers.Contains(s)).ToList();

        return new HoldingsValidationResult<string>(foundHoldings, invalidTickers);
    }
}

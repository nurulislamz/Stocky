using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Portfolio;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;
using stockyapi.Repository.PortfolioRepository.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.PortfolioRepository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PortfolioRepository> _logger;

    public PortfolioRepository(ApplicationDbContext dbContext, ILogger<PortfolioRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PortfolioModel> GetPortfolioFromUserIdAsync(Guid userId, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
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
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }

        var allHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolio.Id).ToListAsync(ct);
        return new PortfolioWithHoldings(portfolio.CashBalance, portfolio.TotalValue, portfolio.InvestedAmount, allHoldings);
    }

    // TODO: Turn the list<guid> requestedIds to a HashSet
    public async Task<HoldingsValidationResult<Guid>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedIds, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new NullReferenceException($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }

        return await ValidateHoldingsExist(portfolioId, requestedIds, ct);
    }

    public async Task<HoldingsValidationResult<string>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new Exception($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }

        return await ValidateHoldingsExist(portfolioId, requestedTickers, ct);
    }

    public async Task<(AssetTransactionModel transaction, PortfolioModel updatedPortfolio)> BuyHoldingAsync(Guid userId, BuyOrderCommand command,
        CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new Exception($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }

        var totalCost = command.Quantity * command.Price;

        // Update portfolio balance
        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;

        // Update or create stock holding
        var existingHolding = _dbContext.StockHoldings.SingleOrDefault(holding => holding.PortfolioId == portfolio.Id && holding.Ticker == command.Ticker);

        decimal newTotalShares = (existingHolding?.Shares ?? 0) + command.Quantity;
        decimal newTotalCost = ((existingHolding?.AverageCost ?? 0) * (existingHolding?.Shares ?? 0)) + totalCost;
        decimal newAverageCost = newTotalShares > 0 ? newTotalCost / newTotalShares : command.Price;
        if (existingHolding != null)
        {
            // Update existing holding
            existingHolding.Shares = newTotalShares;
            existingHolding.AverageCost = newAverageCost;
        }
        else
        {
            // Create new holding
            await _dbContext.StockHoldings.AddAsync(new StockHoldingModel
            {
                PortfolioId = portfolio.Id,
                Ticker = command.Ticker,
                Shares = newTotalShares,
                AverageCost = newAverageCost,
            }, ct);
        }

        // Save transaction and update portfolio
        var transaction = new AssetTransactionModel
        {
            PortfolioId = command.PortfolioId,
            Ticker = command.Ticker,
            Type = TransactionType.Buy,
            Quantity = command.Quantity,
            NewAverageCost = newAverageCost,
            Price = command.Price,
        };

        await _dbContext.AssetTransactions.AddAsync(transaction, ct);
        await _dbContext.SaveChangesAsync(ct);

        return (transaction, portfolio);
    }

    public async Task<(AssetTransactionModel transaction, PortfolioModel updatedPortfolio)> SellHoldingAsync(Guid userId, SellOrderCommand command,
        CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio is null)
        {
            var exception = new Exception($"Portfolio not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }

        var totalProceeds = command.Quantity * command.Price;

        // Update portfolio balance
        portfolio.CashBalance += totalProceeds;
        portfolio.InvestedAmount -= totalProceeds;

        // Update or create stock holding
        var holding = await _dbContext.StockHoldings.SingleAsync(holding => holding.PortfolioId == portfolio.Id & holding.Ticker == command.Ticker, ct);

        // Update holding
        var newTotalShares =  holding.Shares - command.Quantity;
        var newTotalCost = ( holding.AverageCost *  holding.Shares);
        var newAverageCost = newTotalShares == 0 ? 0 : newTotalCost / newTotalShares;

        // Logic to delete holding if newTotalShares == 0
        if (newTotalShares == 0)
        {
            _dbContext.StockHoldings.Remove( holding);
        }

         holding.Shares = newTotalShares;
         holding.AverageCost = newAverageCost;

        // Save transaction and update portfolio
        var transaction = new AssetTransactionModel
        {
            PortfolioId = command.PortfolioId,
            Ticker = command.Ticker,
            Type = TransactionType.Sell,
            Quantity = command.Quantity,
            NewAverageCost = newAverageCost,
            Price = command.Price,
        };

        await _dbContext.AssetTransactions.AddAsync(transaction, ct);
        await _dbContext.SaveChangesAsync(ct);

        return (transaction, portfolio);
    }

    // TODO: Create a parameter and system to allow one to be reimbursed the total funds of the deleted assets
    public async Task<List<AssetTransactionModel>> DeleteHoldingsAsync(Guid userId, List<StockHoldingModel> holdings, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
        {
            var exception = new Exception($"PortfolioId not found. UserId {userId} must be wrong or something went wrong during setup.");
            _logger.LogError(new EventId(123, "Failed to find Portfolio"), exception, "Failed to find Portfolio");
            throw exception;
        }


        var deletedTransaction = holdings.Select(h => new AssetTransactionModel
        {
            PortfolioId = portfolioId,
            Ticker = h.Ticker,
            Type = TransactionType.Delete,
            Quantity = null,
            NewAverageCost = null,
            Price = null,
        }).ToList();

        _dbContext.StockHoldings.RemoveRange(holdings);
        await _dbContext.AssetTransactions.AddRangeAsync(deletedTransaction, ct);

        await _dbContext.SaveChangesAsync(ct);
        return deletedTransaction;
    }

    // helper function, returns a list of ids found and not found, same for tickers
    private async Task<HoldingsValidationResult<Guid>> ValidateHoldingsExist(Guid portfolioId, Guid[] requestedIds, CancellationToken ct)
    {
        var foundHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .Where(h => requestedIds.Contains(h.Id))
            .ToListAsync(ct);

        var foundHoldingIds = foundHoldings.Select(g => g.Id);
        var invalidIds = requestedIds.Where(s => !foundHoldingIds.Contains(s)).ToList();

        return new HoldingsValidationResult<Guid>(foundHoldings, invalidIds);
    }

    private async Task<HoldingsValidationResult<string>> ValidateHoldingsExist(Guid portfolioId, string[] requestedTickers, CancellationToken ct)
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

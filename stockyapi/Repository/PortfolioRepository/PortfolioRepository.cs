using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Portfolio;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.PortfolioRepository;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PortfolioRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PortfolioModel>> GetPortfolioModelFromUserIdAsync(Guid userId, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio == null)
            return new NotFoundFailure404("Portfolio not found");
        
        return Result<PortfolioModel>.Success(portfolio);
    }
    
    public async Task<Result<PortfolioWithHoldings>> ListAllHoldingsAsync(Guid userId, CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .SingleOrDefaultAsync(ct);
        
        if (portfolio == null)
            return new NotFoundFailure404("Portfolio not found. UserId must be wrong.");
        
        var allHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolio.Id).ToListAsync(ct);
        return Result<PortfolioWithHoldings>.Success(new PortfolioWithHoldings(portfolio.CashBalance,
            portfolio.TotalValue, portfolio.InvestedAmount, allHoldings));
    }
    
    // TODO: Turn the list<guid> requestedIds to a HashSet
    public async Task<Result<List<StockHoldingModel>>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedIds, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
            return new NotFoundFailure404("Could not find the user portfolio. Something is wrong with your UserId");

        var holdingsResponse = await ValidateHoldingsExist(portfolioId, requestedIds, ct);
        if (holdingsResponse.invalidIds.Any() )
            return new ValidationFailure422($"These Ids were not valid: {string.Join(", ", holdingsResponse.invalidIds.ToString())}");
        
        return Result<List<StockHoldingModel>>.Success(holdingsResponse.validHoldings);
    }

    public async Task<Result<List<StockHoldingModel>>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
            return new NotFoundFailure404("Could not find the user portfolio. Something is wrong with your UserId");

        var holdingsResponse = await ValidateHoldingsExist(portfolioId, requestedTickers, ct);
        if (holdingsResponse.invalidIds.Any() )
            return new ValidationFailure422($"These Ids were not valid: {string.Join(", ", holdingsResponse.invalidIds.ToString())}");
        
        return Result<List<StockHoldingModel>>.Success(holdingsResponse.validHoldings);
    }

    public async Task<Result<(AssetTransactionModel, PortfolioModel)>> BuyHoldingAsync(Guid userId, BuyOrderCommand command,
        CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio == null)
            return new NotFoundFailure404("Portfolio not found");
        
        var totalCost = command.Quantity * command.Price;

        // Update portfolio balance
        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;

        // Update or create stock holding
        var existingHolding = _dbContext.StockHoldings.SingleOrDefault(holding => holding.PortfolioId == portfolio.Id);
        
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

        if (portfolio.CashBalance - totalCost < 0)
            return new ValidationFailure422("The buy Transaction would have reduced the cashBalance below 0.");
        
        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;
        
        await _dbContext.AssetTransactions.AddAsync(transaction, ct);
        await _dbContext.SaveChangesAsync(ct);

        return (transaction, portfolio);
    }

    public async Task<Result<(AssetTransactionModel, PortfolioModel)>> SellHoldingAsync(Guid userId, SellOrderCommand command,
        CancellationToken ct)
    {
        var portfolio = await _dbContext.Portfolios.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (portfolio == null)
            return new NotFoundFailure404("Portfolio not found");
        
        var totalCost = command.Quantity * command.Price;

        // Update portfolio balance
        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;

        // Update or create stock holding
        var existingHolding = _dbContext.StockHoldings.SingleOrDefault(holding => holding.PortfolioId == portfolio.Id);
        if (existingHolding == null)
            return new BadRequestFailure400("Stock is not owned. Error in repo should not occur");
        
        // Update holding
        var newTotalShares = existingHolding.Shares - command.Quantity;
        var newTotalCost = (existingHolding.AverageCost * existingHolding.Shares);
        var newAverageCost = newTotalShares == 0 ? 0 : newTotalCost / newTotalShares;
        
        // Logic to delete holding if newTotalShares == 0
        if (newTotalShares == 0)
        {
            _dbContext.StockHoldings.Remove(existingHolding);
        }
        
        existingHolding.Shares = newTotalShares;
        existingHolding.AverageCost = newAverageCost;

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

    public async Task<Result<IEnumerable<StockHoldingModel>>> DeleteHoldingsByIdAsync(Guid userId, Guid[] requestedHoldingsIds, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
            return new NotFoundFailure404("Could not find the user portfolio. Something is wrong with your UserId");

        var holdingsResponse = await ValidateHoldingsExist(portfolioId, requestedHoldingsIds, ct);
        if (holdingsResponse.invalidIds.Any() )
            return new ValidationFailure422($"These Ids were not valid: {string.Join(", ", holdingsResponse.invalidIds.ToString())}");

        _dbContext.StockHoldings.RemoveRange(holdingsResponse.validHoldings);

        await _dbContext.SaveChangesAsync(ct);

        return holdingsResponse.validHoldings;
    }

    public async Task<Result<List<StockHoldingModel>>> DeleteHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken ct)
    {
        var portfolioId = await _dbContext.Portfolios
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .SingleOrDefaultAsync(ct);

        if (portfolioId == Guid.Empty)
            return new NotFoundFailure404("Could not find the user portfolio. Something is wrong with your UserId");

        var holdingsResponse = await ValidateHoldingsExist(portfolioId, requestedTickers, ct);
        if (holdingsResponse.invalidIds.Any() )
            return new ValidationFailure422($"These Ids were not valid: {string.Join(", ", holdingsResponse.invalidIds.ToString())}");

        _dbContext.StockHoldings.RemoveRange(holdingsResponse.validHoldings);
        await _dbContext.SaveChangesAsync(ct);

        return holdingsResponse.validHoldings;
    }

    // helper function, returns a list of ids found and not found, same for tickers
    private async Task<(List<StockHoldingModel> validHoldings, IEnumerable<Guid> invalidIds)> ValidateHoldingsExist(Guid portfolioId, Guid[] requestedIds, CancellationToken ct)
    {
        var foundHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .Where(h => requestedIds.Contains(h.Id))
            .ToListAsync(ct);

        var foundHoldingIds = foundHoldings.Select(g => g.Id);
        var invalidIds = requestedIds.Where(s => foundHoldingIds.Contains(s));

        return (foundHoldings, invalidIds);
    }

    private async Task<(List<StockHoldingModel> validHoldings, IEnumerable<string> invalidIds)> ValidateHoldingsExist(Guid portfolioId, string[] requestedTickers, CancellationToken ct)
    {
        var foundHoldings = await _dbContext.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .Where(h => requestedTickers.Contains(h.Ticker))
            .ToListAsync(ct);

        var foundTickers = foundHoldings.Select(g => g.Ticker);
        var invalidTickers = requestedTickers.Where(s => foundTickers.Contains(s));

        return (foundHoldings, invalidTickers);
    }
}

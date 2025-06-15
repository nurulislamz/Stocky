using Microsoft.EntityFrameworkCore;
using stockyapi.Repository.Portfolio;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Repository.Portfolio;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly ApplicationDbContext _context;

    public PortfolioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PortfolioModel?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Portfolios
            .Where(x => x.UserId == userId)
            .Include(p => p.StockHoldings)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync();
    }

    public async Task<PortfolioModel?> GetByIdAsync(Guid id)
    {
        return await _context.Portfolios
            .Where(x => x.Id == id)
            .Include(p => p.StockHoldings)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync();
    }

    public async Task<PortfolioModel> CreateAsync(PortfolioModel portfolio)
    {
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();
        return portfolio;
    }

    public async Task UpdateAsync(PortfolioModel portfolio)
    {
        _context.Portfolios.Update(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var portfolio = await GetByIdAsync(id);
        if (portfolio != null)
        {
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<StockHoldingModel?> GetHoldingAsync(Guid portfolioId, string symbol)
    {
        return await _context.StockHoldings
            .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.Symbol == symbol);
    }

    public async Task<IEnumerable<StockHoldingModel>> GetHoldingsAsync(Guid portfolioId)
    {
        return await _context.StockHoldings
            .Where(h => h.PortfolioId == portfolioId)
            .ToListAsync();
    }

    public async Task<StockHoldingModel> AddHoldingAsync(StockHoldingModel holding)
    {
        _context.StockHoldings.Add(holding);
        await _context.SaveChangesAsync();
        return holding;
    }

    public async Task UpdateHoldingAsync(StockHoldingModel holding)
    {
        _context.StockHoldings.Update(holding);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteHoldingAsync(Guid holdingId)
    {
        var holding = await _context.StockHoldings.FindAsync(holdingId);
        if (holding != null)
        {
            _context.StockHoldings.Remove(holding);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<TransactionModel> AddTransactionAsync(TransactionModel transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<IEnumerable<TransactionModel>> GetTransactionsAsync(Guid portfolioId)
    {
        return await _context.Transactions
            .Where(t => t.PortfolioId == portfolioId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TransactionModel?> GetTransactionAsync(Guid transactionId)
    {
        return await _context.Transactions.FindAsync(transactionId);
    }

    public async Task<decimal> GetTotalValueAsync(Guid portfolioId)
    {
        var portfolio = await GetByIdAsync(portfolioId);
        return portfolio?.TotalValue ?? 0;
    }

    public async Task<decimal> GetCashBalanceAsync(Guid portfolioId)
    {
        var portfolio = await GetByIdAsync(portfolioId);
        return portfolio?.CashBalance ?? 0;
    }

    public async Task<decimal> GetInvestedAmountAsync(Guid portfolioId)
    {
        var portfolio = await GetByIdAsync(portfolioId);
        return portfolio?.InvestedAmount ?? 0;
    }

    public async Task UpdatePortfolioValueAsync(Guid portfolioId, decimal totalValue, decimal cashBalance, decimal investedAmount)
    {
        var portfolio = await GetByIdAsync(portfolioId);
        if (portfolio != null)
        {
            portfolio.TotalValue = totalValue;
            portfolio.CashBalance = cashBalance;
            portfolio.InvestedAmount = investedAmount;
            await UpdateAsync(portfolio);
        }
    }
}

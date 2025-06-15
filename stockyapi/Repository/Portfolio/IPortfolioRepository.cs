using stockymodels.models;

namespace stockyapi.Repository.Portfolio;

public interface IPortfolioRepository
{
    // Portfolio Operations
    Task<PortfolioModel?> GetByUserIdAsync(Guid userId);
    Task<PortfolioModel?> GetByIdAsync(Guid id);
    Task<PortfolioModel> CreateAsync(PortfolioModel portfolio);
    Task UpdateAsync(PortfolioModel portfolio);
    Task DeleteAsync(Guid id);

    // Stock Holdings Operations
    Task<StockHoldingModel?> GetHoldingAsync(Guid portfolioId, string symbol);
    Task<IEnumerable<StockHoldingModel>> GetHoldingsAsync(Guid portfolioId);
    Task<StockHoldingModel> AddHoldingAsync(StockHoldingModel holding);
    Task UpdateHoldingAsync(StockHoldingModel holding);
    Task DeleteHoldingAsync(Guid holdingId);

    // Transaction Operations
    Task<TransactionModel> AddTransactionAsync(TransactionModel transaction);
    Task<IEnumerable<TransactionModel>> GetTransactionsAsync(Guid portfolioId);
    Task<TransactionModel?> GetTransactionAsync(Guid transactionId);

    // Portfolio Value Operations
    Task<decimal> GetTotalValueAsync(Guid portfolioId);
    Task<decimal> GetCashBalanceAsync(Guid portfolioId);
    Task<decimal> GetInvestedAmountAsync(Guid portfolioId);
    Task UpdatePortfolioValueAsync(Guid portfolioId, decimal totalValue, decimal cashBalance, decimal investedAmount);
}

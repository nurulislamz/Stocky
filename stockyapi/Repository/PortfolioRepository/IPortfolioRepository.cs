using stockyapi.Application.Portfolio;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;
using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository;

public interface IPortfolioRepository
{
    // Portfolio Operations
    Task<PortfolioModel> GetPortfolioFromUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortfolioWithHoldings> ListAllHoldingsAsync(Guid userId, CancellationToken cancellationToken);
    Task<HoldingsValidationResult<Guid>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedHoldingsIds, CancellationToken cancellationToken);
    Task<HoldingsValidationResult<string>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken cancellationToken);
    Task<(AssetTransactionModel transaction, PortfolioModel updatedPortfolio)> BuyHoldingAsync(Guid userId, BuyOrderCommand command, CancellationToken cancellationToken);
    Task<(AssetTransactionModel transaction, PortfolioModel updatedPortfolio)> SellHoldingAsync(Guid userId, SellOrderCommand command, CancellationToken cancellationToken);
    Task<List<AssetTransactionModel>> DeleteHoldingsAsync(Guid userId, List<StockHoldingModel> holdings, CancellationToken cancellationToken);
}

using stockyapi.Application.Portfolio;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.Funds.Types;
using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository;

public interface IPortfolioRepository
{
    // Portfolio Operations
    Task<Result<PortfolioModel>> GetPortfolioModelFromUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<PortfolioWithHoldings>> ListAllHoldingsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<List<StockHoldingModel>>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedHoldingsIds, CancellationToken cancellationToken);
    Task<Result<List<StockHoldingModel>>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken cancellationToken);
    Task<Result<(AssetTransactionModel, PortfolioModel)>> BuyHoldingAsync(Guid userId, BuyOrderCommand command, CancellationToken cancellationToken);
    Task<Result<(AssetTransactionModel, PortfolioModel)>> SellHoldingAsync(Guid userId, SellOrderCommand command, CancellationToken cancellationToken);
    Task<Result<IEnumerable<StockHoldingModel>>> DeleteHoldingsByIdAsync(Guid userId, Guid[] requestedHoldingsIds, CancellationToken cancellationToken);
    Task<Result<List<StockHoldingModel>>> DeleteHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken cancellationToken);
}

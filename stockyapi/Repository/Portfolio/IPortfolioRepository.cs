using stockyapi.Application.Commands.Portfolio;
using stockyapi.Repository.Funds.Types;
using stockyapi.Repository.PortfolioRepository.Types;
using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository;

public interface IPortfolioRepository
{
    Task<PortfolioModel> GetPortfolioFromUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<PortfolioWithHoldings> ListAllHoldingsAsync(Guid userId, CancellationToken cancellationToken);
    Task<HoldingsValidationResult<Guid>> GetHoldingsByIdAsync(Guid userId, Guid[] requestedHoldingsIds, CancellationToken cancellationToken);
    Task<HoldingsValidationResult<string>> GetHoldingsByTickerAsync(Guid userId, string[] requestedTickers, CancellationToken cancellationToken);
    Task<TradeResult> BuyHoldingAsync(Guid userId, StockBoughtCommand command, CancellationToken cancellationToken);
    Task<TradeResult> SellHoldingAsync(Guid userId, StockSoldCommand command, CancellationToken cancellationToken);
    Task<List<Guid>> DeleteHoldingsAsync(Guid userId, List<StockHoldingModel> holdings, CancellationToken cancellationToken);
}

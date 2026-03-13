using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository.Types;

public record TradeResult(
    long EventId,
    string Ticker,
    decimal Quantity,
    decimal Price,
    decimal Delta,
    decimal NewAverageCost,
    PortfolioModel UpdatedPortfolio);

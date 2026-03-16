using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository.Types;

public record HoldingsValidationResult<T>(
    List<StockHoldingAggregate> Holdings,
    List<T> MissingIdsOrTickers);

using stockymodels.models;

namespace stockyapi.Repository.PortfolioRepository.Types;

public record HoldingsValidationResult<T>(
    List<StockHoldingModel> Holdings,
    List<T> MissingIdsOrTickers);

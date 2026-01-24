using stockymodels.models;

namespace stockyapi.Repository.Funds.Types;

public record HoldingsValidationResult<T>(
    List<StockHoldingModel> Holdings,
    List<T> MissingIdsOrTickers);

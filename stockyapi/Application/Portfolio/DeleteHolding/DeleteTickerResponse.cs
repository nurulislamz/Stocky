using stockyapi.Application.Portfolio.ZHelperTypes;

namespace stockyapi.Application.Portfolio.DeleteHolding;

public record DeleteHoldingsResponse(IEnumerable<DeleteConfirmationDto> DeletedHoldings);
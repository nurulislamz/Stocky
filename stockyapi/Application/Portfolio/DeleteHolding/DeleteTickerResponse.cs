using System.Collections.Immutable;
using stockyapi.Application.Portfolio;

namespace stockyapi.Responses;

public class DeleteHoldingsResponse(IEnumerable<DeleteConfirmationDto> deletedHoldings);
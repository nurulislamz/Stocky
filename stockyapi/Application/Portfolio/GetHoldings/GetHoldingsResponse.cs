using System.Collections.Immutable;
using stockyapi.Application.Portfolio;

namespace stockyapi.Responses;
public record GetHoldingsResponse(ImmutableArray<HoldingDto> Items);


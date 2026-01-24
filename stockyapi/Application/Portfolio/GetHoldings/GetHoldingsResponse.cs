using System.Collections.Immutable;
using stockyapi.Application.Portfolio.ZHelperTypes;

namespace stockyapi.Application.Portfolio.GetHoldings;
public record GetHoldingsResponse(ImmutableArray<HoldingDto> Items);


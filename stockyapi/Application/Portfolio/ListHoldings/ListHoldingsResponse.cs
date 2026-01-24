using System.Collections.Immutable;
using stockyapi.Application.Portfolio.ZHelperTypes;

namespace stockyapi.Application.Portfolio.ListHoldings;

public record ListHoldingsResponse(
    decimal TotalValue,
    decimal CashBalance,
    decimal InvestedAmount,
    ImmutableArray<HoldingDto> Items);


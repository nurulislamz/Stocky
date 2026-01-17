using System.Collections.Immutable;
using stockyapi.Application.Portfolio;

namespace stockyapi.Responses;

public record ListHoldingsResponse(
    decimal TotalValue,
    decimal CashBalance,
    decimal InvestedAmount,
    ImmutableArray<HoldingDto> Items);


using stockymodels.models;

namespace stockymodels.Events.Funds;

/// <summary>Cash out from buy (fund stream only).</summary>
public record FundsRemovedStockyEvent : FundAccountEvent
{
    public required decimal Amount { get; init; }
    public required decimal CashBalanceBefore { get; init; }
    public required decimal CashBalanceAfter { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(FundAccountAggregate fundAccount)
    {
        fundAccount.CashBalance = CashBalanceAfter;
        fundAccount.UpdatedAt = OccurredAt.UtcDateTime;
    }
}

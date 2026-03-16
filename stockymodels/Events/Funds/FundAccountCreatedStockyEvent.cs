using stockymodels.models;

namespace stockymodels.Events.Funds;

public record FundAccountCreatedStockyEvent : FundAccountEvent
{
    public required decimal InitialCashBalance { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }

    public override void Apply(FundAccountAggregate fundAccount)
    {
        fundAccount.CashBalance = InitialCashBalance;
        fundAccount.UpdatedAt = OccurredAt.UtcDateTime;
    }
}

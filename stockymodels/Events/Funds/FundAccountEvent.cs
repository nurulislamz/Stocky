using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.Events.Funds;

/// <summary>
/// Base for events on the fund stream (AggregateType FundId, AggregateId = FundAccountId).
/// Applies to FundAccountAggregate only; no portfolio or holding logic.
/// </summary>
public abstract record FundAccountEvent : StockyEventPayload, IEventApplier<FundAccountAggregate>
{
    public abstract void Apply(FundAccountAggregate fundAccount);
}

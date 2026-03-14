using stockymodels.Models.Enums;

namespace stockymodels.Events;

public record StockyEvent(AggregateType AggregateType, Guid AggregateId, StockyEventPayload Payload);

public abstract record StockyEventPayload;
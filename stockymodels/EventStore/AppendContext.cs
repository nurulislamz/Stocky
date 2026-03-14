namespace stockymodels.EventStore;

/// <summary>
/// Caller-level metadata shared by both command and event inserts.
/// Domain-specific data (AggregateId, OccurredAt, RequestId) lives on the event itself.
/// </summary>
public record AppendContext(
    Guid UserId,
    Guid TraceId
);

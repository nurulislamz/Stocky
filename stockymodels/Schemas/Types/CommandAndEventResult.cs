
public sealed record CommandAndEventResult(
  Guid CommandId,
  Guid CommandUserId,
  Guid CommandTraceId,
  DateTimeOffset CommandDbStoredAtTime,
  Guid EventId,
  Guid EventUserId,
  Guid EventAggregateId,
  int EventAggregateSequenceId,
  Guid EventCommandId,
  Guid EventTraceId,
  DateTimeOffset EventDbStoredAtTime
);
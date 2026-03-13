using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.Event;

/// <summary>
/// Append-only event store. Events are immutable; only appends are allowed.
/// Two-prong flow: call CreateEvent (command → event), then Add(evt), then caller commits read-model changes and SaveChanges in one transaction.
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Creates a new event ready for persistence. Payload is serialized as JSON (command or rich event).
    /// </summary>
    /// <param name="userId">The user who owns/triggered this event (for RLS and audit).</param>
    /// <param name="aggregateType">Aggregate domain type the event belongs to (e.g. User or Portfolio).</param>
    /// <param name="aggregateId">Aggregate identifier this event applies to.</param>
    /// <param name="sequenceId">Per-aggregate sequence number for ordering.</param>
    /// <param name="eventType">Domain event type.</param>
    /// <param name="payload">Command or rich event; will be serialized to EventPayloadJson.</param>
    /// <param name="traceId">Optional correlation id for tracing.</param>
    /// <param name="commandId">Optional command that produced this event.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>An <see cref="EventModel"/> with timestamps set; Id is auto-generated on insert.</returns>
    EventModel CreateEvent(
        Guid userId,
        AggregateType aggregateType,
        Guid aggregateId,
        int sequenceId,
        EventType eventType,
        object payload,
        Guid? traceId = null,
        Guid? commandId = null,
        CancellationToken ct = default);

    /// <summary>
    /// Adds one or more events to the current context. Caller must call SaveChanges to commit; use same transaction as read-model updates.
    /// </summary>
    void Add(params EventModel[] events);

    /// <summary>
    /// Appends a single event and saves immediately. For event + read-model in one transaction, use Add() then SaveChanges from the caller.
    /// </summary>
    Task AppendAsync(EventModel eventModel, CancellationToken cancellationToken = default);
}

using stockymodels.models;

namespace stockyapi.Repository.Event;

/// <summary>
/// Append-only event store. Events are immutable; only appends are allowed.
/// </summary>
public interface IEventRepository
{


    /// <summary>
    /// <summary>
    /// Creates a new event payload object ready for persistence.
    /// </summary>
    /// <typeparam name="TCommand">Command type that represents the event data.</typeparam>
    /// <param name="aggregateType">Aggregate domain type the event belongs to (for example, User or Portfolio).</param>
    /// <param name="aggregateId">Aggregate identifier this event applies to.</param>
    /// <param name="eventType">Domain event type.</param>
    /// <param name="command">Command used to build the event payload.</param>
    /// <returns>
    /// An <see cref="EventModel"/> with a generated Id and timestamps. Database identity values (like sequence numbering) are expected to be assigned on save.
    /// </returns>
    public EventModel CreateEvent(
        AggregateType aggregateType,
        Guid aggregateId,
        EventType eventType,
        Command command,
        CancellationToken ct = default);

    /// <summary>
    /// Adds one or more events to the current context without saving. Use when events must be committed in the same transaction as other changes; the caller must call SaveChanges.
    /// </summary>
    /// <param name="events">One or more events to add. Pass as many as needed: Add(evt1, evt2, evt3, ...).</param>
    Task Add(params EventModel[] events);

    /// <summary>
    /// Appends a single event and saves immediately. Use for standalone event appends. For event + read-model in one transaction, use Add() and then save from the caller.
    /// </summary>
    Task AppendAsync(EventModel eventModel, CancellationToken cancellationToken = default);
}

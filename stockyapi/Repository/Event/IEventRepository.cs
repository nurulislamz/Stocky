using stockymodels.models;

namespace stockyapi.Repository.Event;

/// <summary>
/// Append-only event store. Events are immutable; only appends are allowed.
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Adds one or more events to the current context without saving. Use when events must be committed in the same transaction as other changes; the caller must call SaveChanges.
    /// </summary>
    /// <param name="events">One or more events to add. Pass as many as needed: Add(evt1, evt2, evt3, ...).</param>
    void Add(params EventModel[] events);

    /// <summary>
    /// Appends a single event and saves immediately. Use for standalone event appends. For event + read-model in one transaction, use Add() and then save from the caller.
    /// </summary>
    Task AppendAsync(EventModel eventModel, CancellationToken cancellationToken = default);
}

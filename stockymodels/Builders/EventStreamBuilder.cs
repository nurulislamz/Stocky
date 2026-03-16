using Microsoft.Extensions.Logging;
using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.EventStore;

/// <summary>
/// Blueprint for replaying an event stream into a single aggregate model.
/// Load events for (AggregateType, aggregateId), require first event to be a creation event, then apply the rest in order.
/// </summary>
/// <typeparam name="TEvent">Event payload type (e.g. PortfolioEvent, FundAccountEvent).</typeparam>
/// <typeparam name="TModel">Resulting model type (e.g. PortfolioAggregate, FundAccountAggregate).</typeparam>
/// <typeparam name="TCreateEvent">The creation event type (e.g. PortfolioCreatedStockyEvent).</typeparam>
public abstract class EventStreamBuilder<TEvent, TModel, TCreateEvent>
	where TEvent : StockyEventPayload, IEventApplier<TModel>
	where TModel : class
	where TCreateEvent : TEvent
{
	protected readonly PostgresEventStore EventStore;
	protected readonly ILogger Logger;

	protected EventStreamBuilder(PostgresEventStore eventStore, ILogger logger)
	{
		EventStore = eventStore;
		Logger = logger;
	}

	/// <summary>Aggregate type id used to query the event store (e.g. (int)AggregateType.PortfolioId).</summary>
	protected abstract int AggregateTypeId { get; }

	/// <summary>Build the initial model from the creation event.</summary>
	protected abstract TModel CreateFrom(TCreateEvent creationEvent, Guid aggregateId, Guid tradingAccountId);

	/// <summary>Human-readable name for the creation event type (for error messages).</summary>
	protected virtual string CreationEventName => typeof(TCreateEvent).Name;

	public async Task<TModel?> BuildAsync(Guid aggregateId, Guid tradingAccountId, CancellationToken ct = default)
	{
		var events = await EventStore.QueryAllAggregatedEventsAsync<TEvent>(AggregateTypeId, aggregateId, ct);
		if (events is null or { Length: 0 })
			return null;
		return ApplyEvents(aggregateId, tradingAccountId, events);
	}

	public TModel ApplyEvents(Guid aggregateId, Guid tradingAccountId, TEvent[] events)
	{
		if (events.Length == 0)
			throw new InvalidOperationException("At least one event is required.");

		if (events[0] is not TCreateEvent created)
			throw new InvalidOperationException($"First event must be {CreationEventName}.");

		var model = CreateFrom(created, aggregateId, tradingAccountId);

		foreach (var @event in events.Skip(1))
		{
			if (@event is TCreateEvent)
				throw new InvalidOperationException($"Cannot have multiple {CreationEventName} events.");
			@event.Apply(model);
		}

		return model;
	}
}

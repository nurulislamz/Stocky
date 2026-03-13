using System.Text.Json;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.Event;

/// <summary>
/// Append-only event repository. Only adds events; no update or delete operations.
/// Two-prong: CreateEvent + Add(evt); caller commits with read-model changes in one transaction.
/// </summary>
public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(ApplicationDbContext context, ILogger<EventRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public EventModel CreateEvent(
        Guid userId,
        AggregateType aggregateType,
        Guid aggregateId,
        int sequenceId,
        EventType eventType,
        object payload,
        Guid? traceId = null,
        Guid? commandId = null,
        CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var validTo = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        return new EventModel
        {
            UserId = userId,
            AggregateType = aggregateType,
            AggregateTypeDesc = aggregateType.ToString(),
            AggregateId = aggregateId,
            AggregateVersion = 0,
            SequenceId = sequenceId,
            EventType = eventType,
            EventPayloadJson = JsonSerializer.Serialize(payload),
            TtStart = now,
            TtEnd = now,
            ValidFrom = now,
            ValidTo = validTo,
            CommandId = commandId,
            TraceId = traceId
        };
    }

    /// <inheritdoc />
    public void Add(params EventModel[] events)
    {
        foreach (var evt in events)
            _context.EventModels.Add(evt);
    }

    /// <inheritdoc />
    public async Task AppendAsync(EventModel eventModel, CancellationToken cancellationToken = default)
    {
        Add(eventModel);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Added event {EventType} for aggregate {AggregateId}", eventModel.EventType, eventModel.AggregateId);
    }
}

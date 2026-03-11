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
        AggregateType aggregateType,
        Guid aggregateId,
        int sequenceId,
        EventType eventType,
        object payload,
        Guid? traceId = null,
        CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var validTo = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        return new EventModel
        {
            Id = Guid.NewGuid(),
            AggregateType = aggregateType,
            AggregateId = aggregateId,
            AggregateVersion = 0,
            SequenceId = sequenceId,
            EventType = eventType,
            EventPayloadJson = JsonSerializer.Serialize(payload),
            EventPayloadProtobuf = Array.Empty<byte>(),
            TtStart = now,
            TtEnd = now,
            ValidFrom = now,
            ValidTo = validTo,
            TraceId = traceId ?? Guid.NewGuid()
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
        _logger.LogInformation("Added event {EventId} {EventType}", eventModel.Id, eventModel.EventType);
    }
}

using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;
using Unit = System.ValueTuple;

namespace stockyapi.Repository.Event;

/// <summary>
/// Append-only event repository. Only adds events; no update or delete operations.
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
    public void Add(EventModel eventModel)
    {
        _context.EventModels.Add(eventModel);
    }

    /// <inheritdoc />
    public async Task AppendAsync(EventModel eventModel, CancellationToken cancellationToken = default)
    {
        Add(eventModel);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Added event {EventId} {EventType}", eventModel.Id, eventModel.EventType);
    }
}

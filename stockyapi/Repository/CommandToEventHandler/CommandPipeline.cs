using stockyapi.CommandToEventHandler;
using stockyapi.Middleware;
using stockyapi.Repository.Event;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Repository.CommandToEventHandler;

public record RepositoryResult;

/// <summary>
/// Builds command → events + projection into a single transaction work, then either executes it in one transaction or produces SQL.
/// </summary>
public abstract class CommandToEventPipeline
{
    public IEventRepository EventRepository { get; set; } = null!;
    public abstract stockymodels.Events.Event[] CommandToEvents(Command command);
    public abstract BaseModel UpdateProjection(IEnumerable<stockymodels.Events.Event> events);
    public abstract RepositoryResult ToRepositoryResult(IEnumerable<stockymodels.Events.Event> events, BaseModel model);

    /// <summary>
    /// Builds the full transaction work (events + updated projection) without persisting. Use for SQL generation or to run in one transaction.
    /// </summary>
    public (stockymodels.Events.Event[], BaseModel) HandleCommand(Command command)
    {
        var events = CommandToEvents(command);
        var updatedProjection = UpdateProjection(events);
        return (events, updatedProjection);
    }

    /// <summary>
    /// Runs CommandToEvents → ToEventModels → Add events → UpdateProjection in one database transaction, then returns the result.
    /// </summary>
    public async Task<Result<RepositoryResult>> ExecuteInTransactionAsync(Command command, ApplicationDbContext db, CancellationToken ct = default)
    {
        var (events, projection) = HandleCommand(command);

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var evt in events)
            {
                evt
                db.EventModels.Add();
            }
            db.Update(work.UpdatedProjection);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        var result = ToRepositoryResult(work.DomainEvents, work.UpdatedProjection);
        return Result<RepositoryResult>.Success(result);
    }
}

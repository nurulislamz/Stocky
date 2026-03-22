using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace stockymodels.models;

/// <summary>
/// Append-only log of every command issued against the system.
/// Each command produces one or more events; the relationship is tracked via EventAggregate.CommandId.
/// </summary>
public class CommandAggregate
{
    [Required]
    public Guid CommandId { get; init; }

    /// <summary>The authenticated user who issued this command.</summary>
    [Required]
    public Guid UserId { get; init; }

    /// <summary>Fully qualified command type name (e.g. "UserCreateCommand"). Used for deserialization and auditing.</summary>
    [Required]
    [MaxLength(128)]
    public required string CommandType { get; init; }

    /// <summary>Command body serialized as JSON.</summary>
    [Required]
    public required JsonDocument CommandPayloadJson { get; init; }

    /// <summary>Transaction-time validity start (when the command was issued).</summary>
    [Required]
    public DateTimeOffset TtStart { get; init; }

    /// <summary>Transaction-time validity end (typically MaxValue).</summary>
    [Required]
    public DateTimeOffset TtEnd { get; init; }

    /// <summary>Request/correlation id for idempotency and tracing.</summary>
    [Required]
    public Guid RequestId { get; init; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public Guid? TraceId { get; init; }
}

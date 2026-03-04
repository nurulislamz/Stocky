using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stockymodels.models;

/// <summary>
/// Append-only log of every command issued against the system.
/// Each command produces one or more events; the relationship is tracked via EventModel.CommandId.
/// </summary>
public class CommandModel
{
    [Required]
    [Column("CommandId")]
    public Guid Id { get; set; }

    /// <summary>Fully qualified command type name (e.g. "UserCreateCommand"). Used for deserialization and auditing.</summary>
    [Required]
    [MaxLength(128)]
    public required string CommandType { get; set; }

    /// <summary>Command body serialized as JSON.</summary>
    [Required]
    public required string CommandPayloadJson { get; set; }

    /// <summary>The authenticated user who issued this command.</summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>Correlation id from the originating HTTP request / message.</summary>
    [Required]
    public Guid RequestId { get; set; }

    [Required]
    public DateTimeOffset IssuedAt { get; set; }

    /// <summary>Events produced by this command.</summary>
    public virtual ICollection<EventModel> Events { get; set; } = new List<EventModel>();
}

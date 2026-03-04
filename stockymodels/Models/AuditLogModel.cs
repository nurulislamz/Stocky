using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stockymodels.models;

public class AuditLogModel
{
    [Key]
    [Column("TraceId")]
    public long Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public required string SqlLog { get; set; }

    /// <summary>Command that produced this event. Nullable for events created before the Commands table existed. EF maps this to a shadow FK column CommandId.</summary>
    public CommandModel? Command { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}

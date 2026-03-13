using System.ComponentModel.DataAnnotations;

namespace stockymodels.models;

public class EventModel
{
    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(32)]
    public required string AggregateType { get; set; }

    [Required]
    [MaxLength(32)]
    public required string AggregateTypeDesc { get; set; }

    [Required]
    public Guid AggregateId { get; set; }

    [Required]
    public int SequenceId { get; set; }

    [Required]
    public int AggregateVersion { get; set; }

    [Required]
    [MaxLength(128)]
    public required string EventType { get; set; }

    /// <summary>StockyEvent payload as JSON for querying and APIs.</summary>
    [Required]
    public required string EventPayloadJson { get; set; }

    [Required]
    public DateTimeOffset TtStart { get; set; }

    [Required]
    public DateTimeOffset TtEnd { get; set; }

    [Required]
    public DateTimeOffset ValidFrom { get; set; }

    [Required]
    public DateTimeOffset ValidTo { get; set; }

    /// <summary>FK to the command that produced this event.</summary>
    public Guid? CommandId { get; set; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public Guid? TraceId { get; set; }

    public override string ToString()
    {
        var jsonPreview = EventPayloadJson?.Length > 200
            ? EventPayloadJson[..200] + "..."
            : EventPayloadJson ?? "";
        return $"Id={EventId}, AggregateType={AggregateType}, AggregateId={AggregateId}, SequenceId={SequenceId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadJson={jsonPreview}";
    }
}

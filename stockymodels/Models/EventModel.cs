using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class EventModel
{
    [Required]
    [Column("EventId")]
    public Int64 Id { get; set; }

    [Required]
    public AggregateType AggregateType { get; init; }

    [Required]
    public Guid AggregateId { get; init; }

    [Required]
    public int AggregateVersion { get; set; }

    [Required]
    public int SequenceId { get; init; }

    [Required]
    public required EventType EventType { get; init; }

    /// <summary>StockyEvent payload as JSON for querying and APIs.</summary>
    [Required]
    [StringLength(400)]
    public required string EventPayloadJson { get; init; }

    [Required]
    public required DateTimeOffset TtStart { get; init; }

    [Required]
    public required DateTimeOffset TtEnd { get; init; }

    [Required]
    public required DateTimeOffset ValidFrom { get; init; }

    [Required]
    public required DateTimeOffset ValidTo { get; init; }

    /// <summary>Navigation to the command that produced this event.</summary>
    public CommandModel? Command { get; init; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public Guid? TraceId { get; init; }

    public override string ToString()
    {
        var jsonPreview = EventPayloadJson?.Length > 200
            ? EventPayloadJson[..200] + "..."
            : EventPayloadJson ?? "";
        return $"Id={Id}, AggregateType={AggregateType}, AggregateId={AggregateId}, SequenceId={SequenceId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadJson={jsonPreview}";
    }
}

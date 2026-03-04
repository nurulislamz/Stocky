using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class EventModel
{
    [Required]
    [Column("EventId")]
    public Guid Id { get; set; }

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

    [Required]
    public required byte[] EventPayloadProtobuf { get; set; }

    /// <summary>Event payload as JSON for querying and APIs.</summary>
    [Required]
    public required string EventPayloadJson { get; set; }

    [Required]
    public required DateTimeOffset TtStart { get; init; }

    [Required]
    public required DateTimeOffset TtEnd { get; init; }

    [Required]
    public required DateTimeOffset ValidFrom { get; init; }

    [Required]
    public required DateTimeOffset ValidTo { get; init; }

    /// <summary>FK to the command that produced this event. Nullable for events created before the Commands table existed.</summary>
    public Guid? CommandId { get; set; }

    /// <summary>Navigation to the command that produced this event.</summary>
    public CommandModel? Command { get; set; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public Guid? TraceId { get; init; }

    public override string ToString()
    {
        var jsonPreview = EventPayloadJson?.Length > 200
            ? EventPayloadJson[..200] + "..."
            : EventPayloadJson ?? "";
        return $"Id={Id}, AggregateType={AggregateType}, AggregateId={AggregateId}, SequenceId={SequenceId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadProtobufLength={EventPayloadProtobuf?.Length ?? 0}, EventPayloadJson={jsonPreview}";
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class EventModel
{

    [Required]
    public long SequenceNumber { get; init; }

    [Required]
    [Column("EventId")]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

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

    /// <summary>Event payload serialized as Protobuf (binary).</summary>
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

    public Guid traceId { get; init; }

    public override string ToString() =>
        $"Id={Id}, AggregateType={AggregateType}, AggregateId={AggregateId}, SequenceId={SequenceId}, EventType={EventType}, " +
        $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
        $"EventPayloadProtobufLength={EventPayloadProtobuf?.Length ?? 0}, EventPayloadJson={EventPayloadJson}";
}

using System.ComponentModel.DataAnnotations;

namespace stockymodels.models;

public record EventModel
{
    public required Guid EventId { get; set; }

    public required Guid UserId { get; set; }

    [MaxLength(32)]
    public required int AggregateType { get; set; }

    [MaxLength(32)]
    public required string AggregateTypeDesc { get; set; }

    public required Guid AggregateId { get; set; }

    public int AggregateSequenceId { get; set; }

    [MaxLength(128)]
    public required string EventType { get; set; }

    /// <summary>StockyEvent payload as JSON for querying and APIs.</summary>
    public required string EventPayloadJson { get; set; }

    public required DateTimeOffset TtStart { get; set; }

    public required DateTimeOffset TtEnd { get; set; }

    public required DateTimeOffset ValidFrom { get; set; }

    public required DateTimeOffset ValidTo { get; set; }

    /// <summary>FK to the command that produced this event.</summary>
    public required Guid CommandId { get; set; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public required Guid? TraceId { get; set; }

    public override string ToString()
    {
        var jsonPreview = EventPayloadJson?.Length > 200
            ? EventPayloadJson[..200] + "..."
            : EventPayloadJson ?? "";
        return $"Id={EventId}, AggregateType={AggregateType}, AggregateId={AggregateId}, AggregateSequenceId={AggregateSequenceId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadJson={jsonPreview}";
    }
}

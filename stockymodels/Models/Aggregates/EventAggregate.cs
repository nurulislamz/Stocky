using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using stockymodels.Events;

namespace stockymodels.models;

public record InsertEventAggregate
{
    public required Guid EventId { get; init; }

    public required Guid UserId { get; init; }

    public required string AggregateType { get; init; }

    public required Guid AggregateId { get; init; }

    public required string EventType { get; init; }

    /// <summary>StockyEvent payload as JSON for querying and APIs.</summary>
    public required JsonDocument EventPayloadJson { get; init; }

    public required DateTimeOffset TtStart { get; set; }

    public required DateTimeOffset TtEnd { get; set; }

    public required DateTimeOffset ValidFrom { get; set; }

    public required DateTimeOffset ValidTo { get; set; }

    /// <summary>FK to the command that produced this event.</summary>
    public required Guid CommandId { get; init; }

    /// <summary>Correlation id for distributed tracing. Null when not tracked.</summary>
    public required Guid TraceId { get; init; }

    public override string ToString()
    {
        return $"Id={EventId}, AggregateType={AggregateType}, AggregateId={AggregateId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadJson={EventPayloadJson}";
    }
}

public record InsertEventAggregateWithExpectedNextSequence
{
    public required InsertEventAggregate Event { get; init; }

    public required int ExpectedNextSequence { get; init; }
}

public record InsertEventAggregateWithSeqId : InsertEventAggregate
{

    public override string ToString()
    {
        return $"Id={EventId}, AggregateType={AggregateType}, AggregateId={AggregateId}, AggregateSequenceId={AggregateSequenceId}, EventType={EventType}, " +
               $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
               $"EventPayloadJson={EventPayloadJson}";
    }
}

public record EventAggregate : InsertEventAggregateWithSeqId
{
    public required int AggregateSequenceId { get; init; }

    public required DateTimeOffset StoredAt {get; init; }

    public override string ToString()
    {
        return $"Id={EventId}, AggregateType={AggregateType}, AggregateId={AggregateId}, AggregateSequenceId={AggregateSequenceId}, EventType={EventType}, " +
            $"TtStart={TtStart:O}, TtEnd={TtEnd:O}, ValidFrom={ValidFrom:O}, ValidTo={ValidTo:O}, " +
            $"EventPayloadJson={EventPayloadJson}";
    }
}

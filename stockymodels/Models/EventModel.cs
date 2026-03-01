using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class EventModel
{
    [Required]
    [Column("EventId")]
    public Guid Id { get; set; }

    [Required]
    public AggregateType AggregateType;

    [Required]
    public Guid AggregateId { get; init; }

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
}

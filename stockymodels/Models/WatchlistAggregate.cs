using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class WatchlistAggregate : BaseAggregate
{
    [Required]
    [Column("WatchlistId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(20)]
    public required string Symbol { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? Notes { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal? TargetPrice { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal? StopLoss { get; set; }

    // Navigation property
    public virtual UserAggregate? User { get; set; }
}

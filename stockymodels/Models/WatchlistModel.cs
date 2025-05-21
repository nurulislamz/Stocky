using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

public class WatchlistModel : BaseModel
{
    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string Symbol { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string Notes { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal? TargetPrice { get; set; }

    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal? StopLoss { get; set; }

    // Navigation property
    public virtual UserModel User { get; set; }
}
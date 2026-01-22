using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class PriceAlertModel : BaseModel
{
    [Required]
    [Column("PriceAlertId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [StringLength(20)]
    public required string Symbol { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal TargetPrice { get; set; }

    [Required]
    [StringLength(10)]
    public required string Condition { get; set; }

    public bool IsTriggered { get; set; } = false;

    public DateTime? TriggeredAt { get; set; }

    // Navigation property
    public virtual UserModel? User { get; set; }
}

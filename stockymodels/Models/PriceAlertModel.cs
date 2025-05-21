using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.Models;

public class PriceAlertModel : BaseModel
{
    public int UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string Symbol { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Precision(18,2)]
    public decimal TargetPrice { get; set; }

    [Required]
    [StringLength(10)]
    public string Condition { get; set; }

    public bool IsTriggered { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? TriggeredAt { get; set; }

    // Navigation property
    public virtual UserModel? User { get; set; }
}
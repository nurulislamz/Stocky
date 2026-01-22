using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class StockHoldingModel : BaseModel
{
    [Required]
    [Column("StockHoldingId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Ticker { get; set; }

    [Required]
    [Precision(18, 4)]
    public decimal Shares { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal AverageCost { get; set; }

    // Navigation property
    public virtual PortfolioModel? Portfolio { get; set; }
}

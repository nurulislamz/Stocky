using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class StockHoldingModel : BaseModel
{
    [Required]
    [Column("StockHoldingId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("PortfolioId")]
    public Guid PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Ticker { get; set; }

    [Required]
    public decimal Shares { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal AverageCost { get; set; }

    // Navigation property
    public  virtual PortfolioModel Portfolio { get; set; }
}
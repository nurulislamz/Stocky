using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

public class StockHoldingModel : BaseModel
{
    [Required]
    [ForeignKey("Portfolio")]
    public Guid PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; }

    [Required]
    public decimal Shares { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal AverageCost { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal CurrentPrice { get; set; }

    // Navigation property
    public virtual PortfolioModel Portfolio { get; set; }
}
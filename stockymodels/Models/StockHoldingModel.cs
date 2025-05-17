using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using stockymodels.Models;

public class StockHoldingModel : BaseModel
{
    [Required]
    public int PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; }

    [Required]
    public int Shares { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal AverageCost { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal CurrentPrice { get; set; }

    // Navigation property
    public virtual PortfolioModel Portfolio { get; set; }
}
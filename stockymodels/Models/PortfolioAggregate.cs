using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class PortfolioAggregate : BaseAggregate
{
    [Required]
    [Column("PortfolioId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal TotalValue { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal CashBalance { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal InvestedAmount { get; set; }

    // Navigation properties
    public virtual UserAggregate User { get; set; } = null!;

    public virtual ICollection<StockHoldingAggregate> StockHoldings { get; set; } = new List<StockHoldingAggregate>();
}

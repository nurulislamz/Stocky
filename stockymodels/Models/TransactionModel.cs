using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class TransactionModel : BaseModel
{
    [Required]
    [Column("TransactionId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("PortfolioId")]
    public Guid PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public required string Symbol { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    public decimal Shares { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal TotalAmount { get; set; }

    [Required]
    public TransactionStatus Status { get; set; }

    [Required]
    public OrderType OrderType { get; set; }

    [Precision(18, 2)]
    public decimal? LimitPrice { get; set; }

    [Required]
    public DateTime LastPriceUpdate { get; set; }

    // Navigation property
    public virtual PortfolioModel Portfolio { get; set; }
}
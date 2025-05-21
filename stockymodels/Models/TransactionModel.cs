using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

public class TransactionModel : BaseModel
{
    [Required]
    public int PortfolioId { get; set; }

    [Required]
    [MaxLength(20)]
    public string Symbol { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    public int Shares { get; set; }

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

    // Navigation property
    public virtual PortfolioModel Portfolio { get; set; }
}
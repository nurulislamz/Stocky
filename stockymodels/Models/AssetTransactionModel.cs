using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class AssetTransactionModel : BaseModel
{
    [Required]
    [Column("TransactionId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid PortfolioId { get; init; }

    [Required]
    [MaxLength(20)]
    public required string Ticker { get; init; }

    [Required]
    public TransactionType Type { get; init; }

    [Required]
    [Precision(18, 2)]
    public decimal Quantity { get; init; }

    [Required]
    [Precision(18, 2)]
    public decimal Price { get; init; }
    
    [Required]
    [Precision(18, 2)]
    public decimal NewAverageCost { get; init; }
    
    // Navigation property
    public virtual PortfolioModel? Portfolio { get; init; }
}

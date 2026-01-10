using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class AssetTransactionModel : BaseModel
{
    [Required]
    [Column("TransactionId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("PortfolioId")]
    public Guid PortfolioId { get; init; }

    [MaxLength(20)]
    public required string Ticker { get; init; }

    [Required]
    public required TransactionType Type { get; init; }

    [Required]
    [Precision(18, 2)]
    public required decimal Quantity { get; init; }

    [Required]
    [Precision(18, 2)]
    public required decimal Price { get; init; }
    
    [Required]
    [Precision(18, 2)]
    public required decimal NewAverageCost { get; init; }
    
    // Navigation property
    public virtual PortfolioModel Portfolio { get; init; }
}
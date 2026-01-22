using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class PortfolioModel : BaseModel
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
    public virtual UserModel User { get; set; } = null!;

    public virtual ICollection<StockHoldingModel> StockHoldings { get; set; } = new List<StockHoldingModel>();

    public virtual ICollection<AssetTransactionModel> Transactions { get; set; } = new List<AssetTransactionModel>();
    public virtual ICollection<FundsTransactionModel> Funds { get; set; } = new List<FundsTransactionModel>();
}

using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class PortfolioModel : BaseModel
{
    [Required]
    [Column("PortfolioId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("UserId")]
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
    public virtual required UserModel User { get; set; }

    public virtual required ICollection<StockHoldingModel> StockHoldings { get; set; }

    public virtual required ICollection<TransactionModel> Transactions { get; set; }
}
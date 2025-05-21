using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

public class PortfolioModel : BaseModel
{
    [Required]
    public string UserId { get; set; }

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
    public virtual UserModel User { get; set; }
    public virtual ICollection<StockHoldingModel> StockHoldings { get; set; }
    public virtual ICollection<TransactionModel> Transactions { get; set; }
}
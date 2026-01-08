using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class FundsTransactionModel : BaseModel
{
    [Required]
    [Column("TransactionId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("PortfolioId")]
    public Guid PortfolioId { get; set; }

    [Required]
    public FundOperationType Type { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal CashAmount { get; set; }

    // Navigation property
    public virtual PortfolioModel Portfolio { get; set; }
}
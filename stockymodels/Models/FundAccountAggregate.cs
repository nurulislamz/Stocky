using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stockymodels.models;

public class FundAccountAggregate : BaseAggregate
{
    [Required]
    [Column("FundAccountId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid TradingAccountId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal CashBalance { get; set; }
}

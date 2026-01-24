using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.Funds.AddFunds;

public class DepositFundsRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public required decimal Amount { get; init; }
}
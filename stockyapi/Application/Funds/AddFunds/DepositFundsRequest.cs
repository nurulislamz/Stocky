using System.ComponentModel.DataAnnotations;

namespace stockyapi.Requests;

public class DepositFundsRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public required decimal Amount { get; set; }
}
using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class SetFundsRequest : IRequest<SetFundsResponse>
{
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
    public required decimal Amount { get; set; }
}
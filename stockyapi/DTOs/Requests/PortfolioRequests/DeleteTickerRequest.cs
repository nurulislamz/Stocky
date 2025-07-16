using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class DeleteTickerRequest : IRequest<DeleteTickerResponse>
{
    [Required]
    public required string Symbol { get; set; }
}
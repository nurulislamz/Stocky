using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class BuyTickerRequest : IRequest<BuyTickerResponse>
{
    [Required]
    public required string Symbol { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public required decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public required decimal Price { get; set; }
}
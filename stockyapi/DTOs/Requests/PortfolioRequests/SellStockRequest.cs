using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class SellStockRequest : IRequest<BaseResponse>
{
    [Required]
    public string Symbol { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
}
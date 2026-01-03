using System.ComponentModel.DataAnnotations;

namespace stockyapi.Requests;

public class BuyTickerRequest
{
    private const double MaxValue = 100000;
    
    [Required]
    public required string Symbol { get; set; }

    [Required]
    [Range(0, MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public required decimal Quantity { get; set; }

    [Required]
    [Range(0.01, MaxValue, ErrorMessage = "Price must be greater than 0")]
    public required decimal Price { get; set; }
}
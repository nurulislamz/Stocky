using System.ComponentModel.DataAnnotations;

namespace stockyapi.Requests;

public class SellTickerRequest
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
using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.MarketPricing.GetCurrentPrice;

public class CurrentPriceDataRequest
{
    [Required]
    public required string Ticker { get; set; }
}

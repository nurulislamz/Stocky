using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.MarketPricing.MarketDataResponses;

public class HistoricalPriceDataRequest
{
    [Required]
    public required string Ticker { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

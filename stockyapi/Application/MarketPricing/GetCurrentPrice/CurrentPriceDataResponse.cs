namespace stockyapi.Application.MarketPricing.GetCurrentPrice;

public class CurrentPriceDataResponse;

public class CurrentPriceData
{
  public required string Symbol { get; set; }
  public decimal CurrentPrice { get; set; }
  public DateTime Timestamp { get; set; }
}
namespace stockyapi.Application.MarketPricing.MarketDataResponses;

public class HistoricalPriceDataResponse;

public class HistoricalPriceData
{
  public required string Symbol { get; set; }
  public decimal CurrentPrice { get; set; }
  public DateTime Timestamp { get; set; }
}

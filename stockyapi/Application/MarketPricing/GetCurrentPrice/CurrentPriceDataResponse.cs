namespace stockyapi.Responses;

public class CurrentPriceDataResponse;

public class CurrentPriceData
{
  public string Symbol { get; set; }
  public decimal CurrentPrice { get; set; }
  public DateTime Timestamp { get; set; }
}
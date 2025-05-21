namespace stockyapi.Responses;

public class SellTickerResponse : BaseResponse<SellTickerData>;

public class SellTickerData
{
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalCost { get; set; }
    public decimal RemainingCashBalance { get; set; }
    public DateTime TransactionTime { get; set; }
    public string TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
}
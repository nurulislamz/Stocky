namespace stockyapi.Responses;

public class BuyTickerResponse : BaseResponse<BuyTickerData>;

public class BuyTickerData
{
    public required string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalCost { get; set; }
    public decimal RemainingCashBalance { get; set; }
    public DateTime TransactionTime { get; set; }
    public required string TransactionId { get; set; }
    public TransactionStatus Status { get; set; }
}
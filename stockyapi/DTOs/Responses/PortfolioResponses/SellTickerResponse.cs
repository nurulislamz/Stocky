namespace stockyapi.Responses;

public class SellTickerResponse : BaseResponse<SellTickerData>
{
    
}

public class SellTickerData
{
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercentage { get; set; }
    public decimal NewCashBalance { get; set; }
    public DateTime TransactionTime { get; set; }
}
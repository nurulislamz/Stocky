namespace stockyapi.Responses;

public class PortfolioResponse : BaseResponse
{
    public List<PortfolioItem> Items { get; set; }
    public decimal TotalValue { get; set; }
    public decimal TotalProfitLoss { get; set; }
}

public class PortfolioItem
{
    public string Symbol { get; set; }
    public int Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercentage { get; set; }
    public string LastUpdatedTime { get; set; }
}
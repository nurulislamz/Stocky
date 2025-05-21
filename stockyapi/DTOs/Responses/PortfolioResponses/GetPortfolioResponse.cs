using stockyapi.Requests;

namespace stockyapi.Responses;

public class GetPortfolioResponse : BaseResponse<PortfolioData>;

public class PortfolioData
{
    public List<PortfolioItem>? Items { get; set; }
    public decimal TotalValue { get; set; }
    public decimal CashBalance { get; set; }
    public decimal InvestedAmount { get; set; }
}

public class PortfolioItem
{
    public required string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageBuyPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal TotalValue { get; set; }
    public decimal ProfitLoss { get; set; }
    public decimal ProfitLossPercentage { get; set; }
    public required string LastUpdatedTime { get; set; }
}

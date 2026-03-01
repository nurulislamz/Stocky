namespace stockyapi.Application.Portfolio.ZHelperTypes;

public class TradeConfirmationDto
{
    public required string Ticker { get; set; }
    public required decimal? QuantityBought { get; set; }
    public required decimal? ExecutionPrice { get; set; }
    public required decimal? NewAveragePrice { get; set; }
    public required decimal NewCashBalance { get; set; }
    public required decimal NewInvestedAmount { get; set; }
    public required decimal NewTotalValue { get; set; }
    public required decimal? TotalCost { get; set; }
    public required Guid TransactionId { get; set; }
    public required DateTime Timestamp { get; set; }
}

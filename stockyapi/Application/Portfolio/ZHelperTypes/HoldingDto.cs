namespace stockyapi.Application.Portfolio;

public class HoldingDto
{
    public required string Ticker { get; set; }
    public required decimal Quantity { get; set; }
    public required decimal AverageBuyPrice { get; set; }
    public required decimal TotalCost { get; set; }
    public required DateTime LastUpdatedTime { get; set; }
}

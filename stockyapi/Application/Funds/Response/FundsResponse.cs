namespace stockyapi.Application.Funds.Response;

public class FundsResponse(decimal cashBalance, decimal totalValue, decimal investedAmount)
{
    public decimal CashBalance { get; init; } = cashBalance;
    public decimal TotalValue { get; init; } = totalValue;
    public decimal InvestedAmount { get; init; } = investedAmount;
}
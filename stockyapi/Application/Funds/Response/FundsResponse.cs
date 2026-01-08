namespace stockyapi.Responses;

public class FundsResponse(decimal updatedCashBalance, decimal updatedTotalValue, decimal updatedInvestedAmount)
{
    public decimal UpdatedCashBalance { get; init; } = updatedCashBalance;
    public decimal UpdatedTotalValue { get; init; } = updatedTotalValue;
    public decimal UpdatedInvestedAmount { get; init; } = updatedInvestedAmount;
}
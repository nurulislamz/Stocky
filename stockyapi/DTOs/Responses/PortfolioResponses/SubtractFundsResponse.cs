namespace stockyapi.Responses;

public class SubtractFundsResponse : BaseResponse<SubtractFundsData>;

public class SubtractFundsData
{
    public decimal NewBalance { get; set; }
}
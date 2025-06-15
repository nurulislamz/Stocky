namespace stockyapi.Responses;

public class SetFundsResponse : BaseResponse<SetFundsData>;

public class SetFundsData
{
    public decimal NewBalance { get; set; }
}
namespace stockyapi.Responses;

public class AddFundsResponse : BaseResponse<AddFundsData>;

public class AddFundsData
{
    public decimal NewBalance { get; set; }
}
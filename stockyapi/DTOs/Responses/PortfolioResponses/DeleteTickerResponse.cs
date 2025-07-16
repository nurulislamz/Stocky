namespace stockyapi.Responses;

public class DeleteTickerResponse : BaseResponse<DeleteTickerData>;

public class DeleteTickerData
{
    public string Symbol { get; set; }
}
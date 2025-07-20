namespace stockyapi.Responses;

public class AiResponse : BaseResponse<AiData>
{

}

public class AiData
{
    public string? Response { get; set; }
}
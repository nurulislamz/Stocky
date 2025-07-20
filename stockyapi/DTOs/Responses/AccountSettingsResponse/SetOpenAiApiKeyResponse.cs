namespace stockyapi.Responses;

public class SetOpenAiApiKeyResponse : BaseResponse<SetOpenAiApiKeyData>
{

}

public class SetOpenAiApiKeyData
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
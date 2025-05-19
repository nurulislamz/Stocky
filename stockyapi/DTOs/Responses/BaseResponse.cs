namespace stockyapi.Responses;

public abstract class BaseResponse
{
    public required bool Success { get; set; }
    public string? Error { get; set; }
}
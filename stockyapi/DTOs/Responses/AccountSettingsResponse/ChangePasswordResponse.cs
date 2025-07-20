
namespace stockyapi.Responses;

public class ChangePasswordResponse : BaseResponse<ChangePasswordData>
{

}

public class ChangePasswordData
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}
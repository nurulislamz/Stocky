namespace stockyapi.Responses;

public class LoginResponse : BaseResponse<LoginData>
{

}

public class LoginData
{
    public string? Token { get; set; }
}
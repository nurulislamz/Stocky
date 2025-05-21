using System.ComponentModel.DataAnnotations;

namespace stockyapi.Responses;

public class RegisterResponse : BaseResponse<RegisterData>
{
}

public class RegisterData
{
   public string? Token { get; set; }
}
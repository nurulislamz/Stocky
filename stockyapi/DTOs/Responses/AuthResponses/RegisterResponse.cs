using System.ComponentModel.DataAnnotations;

namespace stockyapi.Responses;

public class RegisterResponse : BaseResponse<RegisterData>
{
}

public class RegisterData
{
   public string? Token { get; set; }
   public string? Email { get; set; }
   public string? UserId { get; set; }
}
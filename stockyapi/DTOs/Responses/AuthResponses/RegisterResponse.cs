using System.ComponentModel.DataAnnotations;

namespace stockyapi.Responses;

public class RegisterResponse : BaseResponse
{
   public string? Token { get; set; }
}
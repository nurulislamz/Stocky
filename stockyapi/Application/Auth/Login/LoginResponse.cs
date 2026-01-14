using System.ComponentModel.DataAnnotations;

namespace stockyapi.Responses;

public class LoginResponse(string token)
{
    [Required]
    private string Token { get; init; } = token;
};
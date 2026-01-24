using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.Auth.Login;

public class LoginRequest
{
   [Required, EmailAddress]
   public required string Email { get; init; }

   [Required, MinLength(8)]
   public required string Password { get; init; }
}
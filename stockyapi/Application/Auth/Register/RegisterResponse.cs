using System.ComponentModel.DataAnnotations;

namespace stockyapi.Responses;

public class RegisterResponse(string token, string email, string userId)
{
   [Required]
   public string Token { get; init; } = token;

   [Required, EmailAddress]
   public string Email { get; init; } = email;

   [Required]
   public string UserId { get; init; } = userId;
}
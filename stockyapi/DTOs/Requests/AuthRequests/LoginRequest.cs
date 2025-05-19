using System.ComponentModel.DataAnnotations;
using MediatR;
using stockyapi.Responses;

namespace stockyapi.Requests;

public class LoginRequest : IRequest<LoginResponse>
{
   [Required]
   [EmailAddress]
   public required string Email { get; set; }

   [Required]
   [MinLength(8)]
   public required string Password { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.Auth.Login;

public class ChangeNameRequest
{
   [StringLength(50, MinimumLength = 1)]
   [RegularExpression(@"^[\p{L}\p{M}\s'-]+$", ErrorMessage = "First name can only contain letters, spaces, hyphens and apostrophes.")]
   public required string FirstName { get; set; }

   [StringLength(50, MinimumLength = 1)]
   [RegularExpression(@"^[\p{L}\p{M}\s'-]+$", ErrorMessage = "Surname can only contain letters, spaces, hyphens and apostrophes.")]
   public required string Surname { get; set; }
}
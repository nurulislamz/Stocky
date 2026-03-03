using System.ComponentModel.DataAnnotations;

namespace stockyapi.Application.Auth.Register;

public class RegisterRequest
{
  /// <summary>Allowed: letters (any language), spaces, hyphens, apostrophes. No digits or symbols.</summary>
  [Required]
  [StringLength(50, MinimumLength = 1)]
  [RegularExpression(@"^[\p{L}\p{M}\s'-]+$", ErrorMessage = "First name can only contain letters, spaces, hyphens and apostrophes.")]
  public required string FirstName { get; set; }

  /// <summary>Allowed: letters (any language), spaces, hyphens, apostrophes. No digits or symbols.</summary>
  [Required]
  [StringLength(50, MinimumLength = 1)]
  [RegularExpression(@"^[\p{L}\p{M}\s'-]+$", ErrorMessage = "Surname can only contain letters, spaces, hyphens and apostrophes.")]
  public required string Surname { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(255)]
  public required string Email { get; set; }

  [Required]
  [MinLength(8)]
  public required string Password { get; set; }
}
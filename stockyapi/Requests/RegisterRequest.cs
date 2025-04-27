using System.ComponentModel.DataAnnotations;
namespace stockyapi.Requests;

public class RegisterRequest
{
  [Required]
  [StringLength(50)]
  public string FirstName { get; set; }

  [Required]
  [StringLength(50)]
  public string Surname { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(255)]
  public string Email { get; set; }

  [Required]
  [MinLength(8)]
  public string Password { get; set; }
}
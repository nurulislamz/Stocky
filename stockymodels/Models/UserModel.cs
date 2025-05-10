using System.ComponentModel.DataAnnotations;

namespace stockyapi.Models;

public class UserModel
{
  [Key]
  public int Id { get; set; }

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
  [DataType(DataType.Password)]
  public string Password { get; set; }
}
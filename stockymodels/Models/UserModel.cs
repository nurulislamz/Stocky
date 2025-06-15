using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace stockymodels.models;

public class UserModel : BaseModel
{

  [Required]
  [Column("UserId")]
  public override Guid Id { get; set; }
  
  [Required]
  [MinLength(8)]
  [MaxLength(50)]
  public required string FirstName { get; set; }

  [Required]
  [MinLength(8)]
  [MaxLength(50)]
  public required string Surname { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(255)]
  public required string Email { get; set; }

  [Required]
  [MinLength(8)]
  [DataType(DataType.Password)]
  public required string Password { get; set; }

  [Required]
  public UserRole Role { get; set; }

  public bool IsActive { get; set; } = true;

  public DateTime? LastLogin { get; set; }
  
  // One-to-One Properties
  public virtual PortfolioModel Portfolio { get; set; }
  public virtual UserPreferencesModel Preferences { get; set; }

  // One-to-Many Properties
  public virtual ICollection<WatchlistModel> Watchlist { get; set; }
  public virtual ICollection<PriceAlertModel> PriceAlerts { get; set; }
}
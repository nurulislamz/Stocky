using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using stockymodels.Models.Enums;

namespace stockymodels.models;

public class UserAggregate : BaseAggregate
{

  [Required]
  [Column("UserId")]
  public override Guid Id { get; set; }

  [Required]
  [MaxLength(50)]
  public required string FirstName { get; set; }

  [Required]
  [MaxLength(50)]
  public required string Surname { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(255)]
  public required string Email { get; set; }

  [Required]
  [MinLength(8)]
  [MaxLength(255)]
  [DataType(DataType.Password)]
  public required string Password { get; set; }

  [Required]
  public UserRole Role { get; set; }

  public bool IsActive { get; set; } = true;

  public DateTime? LastLogin { get; set; }

  // One-to-One Properties
  public virtual PortfolioAggregate? Portfolio { get; set; }
  public virtual UserPreferencesAggregate? Preferences { get; set; }

  // One-to-Many Properties
  public virtual ICollection<WatchlistAggregate> Watchlist { get; set; } = new List<WatchlistAggregate>();
  public virtual ICollection<PriceAlertAggregate> PriceAlerts { get; set; } = new List<PriceAlertAggregate>();
}

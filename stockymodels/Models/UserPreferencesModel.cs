using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models;

namespace stockymodels.models;

public class UserPreferencesModel : BaseModel
{
    [Required]
    [Column("UserPreferencesId")]
    public override Guid Id { get; set; }

    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    public string Theme { get; set; } = "light";

    [Required]
    public string Currency { get; set; } = "USD";

    [Required]
    public string Language { get; set; } = "en";

    [Required]
    public bool EmailNotifications { get; set; } = true;

    [Required]
    public bool PushNotifications { get; set; } = true;

    public bool PriceAlerts { get; set; } = true;
    public bool NewsAlerts { get; set; } = true;

    [Required]
    [StringLength(3)]
    public string DefaultCurrency { get; set; } = "USD";

    [Required]
    [StringLength(10)]
    public string Timezone { get; set; }

    // Navigation property
    public virtual UserModel User { get; set; }
}
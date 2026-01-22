using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using stockymodels.models.eums;

namespace stockymodels.models;

public class UserPreferencesModel : BaseModel
{
    [Required]
    [Column("UserPreferencesId")]
    public override Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public Theme Theme { get; set; } = Theme.Light;

    [Required]
    [MaxLength(10)]
    public DefaultCurrency Currency { get; set; } = eums.DefaultCurrency.GDP;

    [Required]
    [MaxLength(10)]
    public Language Language { get; set; } = Language.English;

    [Required]
    public bool EmailNotifications { get; set; } = true;

    [Required]
    public bool PushNotifications { get; set; } = true;

    public bool PriceAlerts { get; set; } = true;
    public bool NewsAlerts { get; set; } = true;

    [Required]
    [StringLength(10)]
    public string Timezone { get; set; } = "UTC";

    // Navigation property
    public virtual UserModel User { get; set; } = null!;
}

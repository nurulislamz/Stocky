using System.ComponentModel.DataAnnotations;
using stockymodels.models;
using stockymodels.Models;

public class UserPreferencesModel : BaseModel
{
    public int UserId { get; set; }

    [Required]
    public string Theme { get; set; } = "light";

    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool PriceAlerts { get; set; } = true;
    public bool NewsAlerts { get; set; } = true;

    [Required]
    [StringLength(3)]
    public string DefaultCurrency { get; set; } = "USD";

    [Required]
    [StringLength(10)]
    public string Language { get; set; } = "en";

    public string Timezone { get; set; }

    // Navigation property
    public virtual UserModel User { get; set; }
}
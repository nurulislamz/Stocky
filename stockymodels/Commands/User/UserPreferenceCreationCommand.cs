using stockymodels.Models.Enums;

namespace stockyapi.Application.Commands.User;

/// <summary>Command for UserPreferenceCreate event. Used when creating default preferences (e.g. on user registration).</summary>
public record UserPreferenceCreationCommand(
    Guid UserId,
    Theme Theme = Theme.Light,
    DefaultCurrency Currency = DefaultCurrency.GDP,
    Language Language = Language.English,
    bool EmailNotifications = true,
    bool PushNotifications = true,
    bool PriceAlerts = true,
    bool NewsAlerts = true,
    string Timezone = "UTC") : Command;

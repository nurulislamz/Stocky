using stockymodels.Models.Enums;

namespace stockymodels.Events;

public record UserPreferencesCreatedEvent : EventBase
{
    public required Guid UserPreferencesId { get; init; }
    public required Theme Theme { get; init; }
    public required DefaultCurrency Currency { get; init; }
    public required Language Language { get; init; }
    public required bool EmailNotifications { get; init; }
    public required bool PushNotifications { get; init; }
    public required bool PriceAlerts { get; init; }
    public required bool NewsAlerts { get; init; }
    public required string Timezone { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public required Guid RequestId { get; init; }
}

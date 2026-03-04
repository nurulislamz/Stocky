namespace stockymodels.Models.Enums;

public enum EventType
{

    // Rich domain events (payloads hold full context, not deltas)
    HoldingCreation,
    HoldingModification,
    HoldingDeletion,
    PortfolioCreated,
    FundsDeposited,
    FundsWithdrawn,
    UserCreated,
    UserNameChanged,
    UserEmailChanged,
    UserPasswordChanged,
    UserDeleted,
    UserPreferencesCreated,
    WatchlistItemAdded,
    WatchlistItemRemoved,
}

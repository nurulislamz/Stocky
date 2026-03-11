namespace stockymodels.Models.Enums;

public enum EventType
{
    // Legacy (command as payload); can later map to rich event types
    UserCreateEvent,
    UserNameChangeEvent,
    UserEmailChangeEvent,
    UserPasswordChangeEvent,
    UserDeleteEvent,
    // Rich domain events (payloads hold full context, not deltas)
    PortfolioCreatedEvent,

    HoldingCreationEvent,
    HoldingModificationEvent,
    HoldingDeletionEvent,
    //
    FundsDepositedEvent,
    FundsWithdrawnEvent,
    //
    WatchlistAddEvent,
    WatchlistRemoveEvent,
    UserPreferencesCreated,
    //
    WatchlistItemAdded,
    WatchlistItemRemoved,
}

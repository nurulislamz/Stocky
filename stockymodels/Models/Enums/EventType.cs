namespace stockymodels.Models.Enums;

public enum EventType
{
    // Legacy (command as payload); can later map to rich event types
    UserCreateEvent,
    UserCreate = UserCreateEvent,
    UserNameChangeEvent,
    UserEmailChangeEvent,
    UserPasswordChangeEvent,
    UserDeleteEvent,
    // Rich domain events (payloads hold full context, not deltas)
    PortfolioCreatedEvent,

    HoldingCreationEvent,
    HoldingModificationEvent,
    HoldingDeletionEvent,
    // Legacy aliases used by PortfolioRepository
    StockBought,
    StockSold,
    DeleteHolding,
    //
    FundAccountCreatedEvent,
    FundsDepositedEvent,
    FundsWithdrawnEvent,
    FundsAddedEvent,
    FundsRemovedEvent,
    DepositFunds = FundsDepositedEvent,
    WithdrawFunds = FundsWithdrawnEvent,
    //
    WatchlistAddEvent,
    WatchlistRemoveEvent,
    UserPreferencesCreated,
    //
    WatchlistItemAdded,
    WatchlistItemRemoved,
}

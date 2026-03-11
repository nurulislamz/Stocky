# Rich domain events

These events are **rich** (full context), not delta-only. They correlate with commands as follows:

| Command | Rich event(s) |
|--------|----------------|
| `StockBoughtCommand` | `HoldingCreationEvent` (first buy) or `HoldingModificationEvent` (add to existing) |
| `StockSoldCommand` | `HoldingModificationEvent` (partial sell) or `HoldingDeletionEvent` (sold all) |
| `DeleteHoldingCommand` | `HoldingDeletionEvent` |
| `PortfolioCreationCommand` | `PortfolioCreatedEvent` |
| `DepositFundsCommand` | `FundsDepositedEvent` |
| `WithdrawFundsCommand` | `FundsWithdrawnEvent` |
| `UserCreateCommand` | `UserCreatedEvent` |
| `UserNameChangeCommand` | `UserNameChangedEvent` |
| `UserEmailChangeCommand` | `UserEmailChangedEvent` |
| `UserPasswordChangeCommand` | `UserPasswordChangedEvent` |
| `UserDeleteCommand` | `UserDeletedEvent` |
| `UserPreferenceCreationCommand` | `UserPreferencesCreatedEvent` |
| `AddToWatchlistCommand` | `WatchlistItemAddedEvent` |
| `RemoveFromWatchlistCommand` | `WatchlistItemRemovedEvent` |

Serialize these as JSON into `EventModel.EventPayloadJson` (and optionally to `EventPayloadProtobuf`).

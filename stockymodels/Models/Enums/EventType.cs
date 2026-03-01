namespace stockymodels.Models.Enums;

public enum EventType
{
    UserCreate,
    UserNameChange,
    UserEmailChange,
    UserPasswordChange,
    UserDelete,
    StockBought,
    StockSold,
    DeleteHolding,
    DepositFunds,
    WithdrawFunds,
    AddToWatchlist,
    RemoveFromWatchlist
}

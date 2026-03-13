public enum CommandType
{
  // User Commands
  UserCreate,
  UserNameChange,
  UserEmailChange,
  UserPasswordChange,
  UserDelete,

  // Portfolio Commands
  StockBought,
  StockSold,
  UpdateHolding,
  DeleteHolding,

  // Fund Commands
  DepositFunds,
  WithdrawFunds,

  // Watchlist Commands
  AddToWatchlist,
  RemoveFromWatchlist
}
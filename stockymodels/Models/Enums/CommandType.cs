public enum CommandType
{
  // User events
  UserCreate,
  UserNameChange,
  UserEmailChange,
  UserPasswordChange,
  UserDelete,

  // Portfolio Events
  StockBought,
  StockSold,
  UpdateHolding,
  DeleteHolding,
  DepositFunds,
  WithdrawFunds,
  AddToWatchlist,
  RemoveFromWatchlist
}
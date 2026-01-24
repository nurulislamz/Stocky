using stockymodels.Models.Enums;

namespace stockyapi.Application.Funds.CommandAndQueries;
// TODO: Fix namespacing everywhere

public abstract record BaseFundCommand(
    Guid UserId,
    Guid PortfolioId,
    decimal CashDelta,
    FundOperationType OperationType);

public record DepositFundCommands(
    Guid UserId,
    Guid PortfolioId,
    decimal CashDelta) : BaseFundCommand(UserId, PortfolioId, CashDelta, FundOperationType.Deposit);

public record WithdrawOrderCommand(
    Guid UserId,
    Guid PortfolioId,
    decimal CashDelta) : BaseFundCommand(UserId, PortfolioId, CashDelta, FundOperationType.Withdrawal);

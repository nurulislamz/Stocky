namespace stockyapi.Application.Commands.Funds;

/// <summary>Command for WithdrawFunds event. Maps from WithdrawFundsRequest.</summary>
public record WithdrawFundsCommand(decimal Amount);

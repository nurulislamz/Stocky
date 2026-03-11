namespace stockyapi.Application.Commands.Funds;

/// <summary>Command for DepositFunds event. Maps from DepositFundsRequest.</summary>
public record DepositFundsCommand(decimal Amount) : Command;

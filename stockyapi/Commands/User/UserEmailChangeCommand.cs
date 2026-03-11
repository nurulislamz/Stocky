namespace stockyapi.Application.Commands.User;

/// <summary>Command for UserEmailChange event.</summary>
public record UserEmailChangeCommand(string NewEmail) : Command;

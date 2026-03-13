namespace stockyapi.Application.Commands.User;

/// <summary>Command for UserPasswordChange event.</summary>
public record UserPasswordChangeCommand(string NewPassword) : Command;

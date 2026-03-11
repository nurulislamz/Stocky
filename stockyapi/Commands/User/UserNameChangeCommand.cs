namespace stockyapi.Application.Commands.User;

/// <summary>Command for UserNameChange event.</summary>
public record UserNameChangeCommand(
    string FirstName,
    string Surname) : Command;

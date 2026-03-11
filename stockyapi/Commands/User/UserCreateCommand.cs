namespace stockyapi.Application.Commands.User;

/// <summary>Command for UserCreate event. Maps from registration (e.g. RegisterRequest).</summary>
public record UserCreateCommand(
    string FirstName,
    string Surname,
    string Email,
    string Password) : Command;

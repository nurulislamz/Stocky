using System;
using stockyapi.Middleware;

namespace stockytests.Integration;

public sealed class TestUserContext : IUserContext
{
    public TestUserContext(bool isAuthenticated, Guid userId, string email, string firstName, string surname, string role)
    {
        IsAuthenticated = isAuthenticated;
        UserId = userId;
        Email = email;
        FirstName = firstName;
        Surname = surname;
        Role = role;
    }

    public bool IsAuthenticated { get; }
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string Surname { get; }
    public string Role { get; }
}

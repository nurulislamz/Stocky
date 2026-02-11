using System;
using stockyapi.Middleware;

namespace stockytests.Integration;

public sealed class TestUserContext : IUserContext
{
    public TestUserContext(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
        IsAuthenticated = true;
    }

    public bool IsAuthenticated { get; }
    public Guid UserId { get; }
    public string Email { get; }
}

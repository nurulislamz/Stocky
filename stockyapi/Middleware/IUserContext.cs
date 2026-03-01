namespace stockyapi.Middleware;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string Email { get; }
    string FirstName { get; }
    string Surname { get; }
    string Role { get; }
}


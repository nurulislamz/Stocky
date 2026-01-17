namespace stockyapi.Middleware;

public interface IUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; } // non-null; web layer guarantees auth
    string Email { get; } // non-null; web layer guarantees auth
    
    // TODO: Add the other claims here
}


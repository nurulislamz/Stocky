using Microsoft.Extensions.Options;
using stockyapi.Application.Auth;
using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Middleware;
using stockyapi.Options;
using stockyapi.Repository.User;
using stockyapi.Services;
using stockymodels.Data;
using stockytests.Helpers;

namespace stockytests.Integration;

[TestFixture]
public class AuthIntegrationTests
{
    private static AuthenticationApi CreateAuthenticationApi(ApplicationDbContext context)
    {
        var jwtSettings = new JwtSettings
        {
            Key = "your-super-secret-key-that-is-long-enough",
            Issuer = "your-issuer",
            Audience = "your-audience",
            ExpirationInMinutes = 60
        };
        var options = Options.Create(jwtSettings);

        var tokenService = new TokenService(options);
        var userRepository = new UserRepository(context);

        return new AuthenticationApi(tokenService, userRepository);
    }

    [Test]
    public async Task Register_ValidRequest_CreatesUserAndReturnsToken()
    {
        // Arrange
        await using var session = await SqliteTestSession.CreateAsync();
        var authApi = CreateAuthenticationApi(session.Context);
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            Surname = "User"
        };

        // Act
        var result = await authApi.Register(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Token, Is.Not.Empty);
        
        var user = await session.Context.Users.FindAsync(Guid.Parse(result.Value.UserId));
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo("test@example.com"));
    }
    
    [Test]
    public async Task Register_ExistingEmail_ReturnsConflict()
    {
        // Arrange
        await using var session = await SqliteTestSession.CreateAsync();
        var authApi = CreateAuthenticationApi(session.Context);
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            Surname = "User"
        };
        await authApi.Register(request, CancellationToken.None);

        // Act
        var result = await authApi.Register(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.TypeOf<ConflictFailure409>());
    }

    [Test]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        // Arrange
        await using var session = await SqliteTestSession.CreateAsync();
        var authApi = CreateAuthenticationApi(session.Context);
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            Surname = "User"
        };
        await authApi.Register(registerRequest, CancellationToken.None);
        
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await authApi.Login(loginRequest, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Token, Is.Not.Empty);
    }
    
    [Test]
    public async Task Login_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        await using var session = await SqliteTestSession.CreateAsync();
        var authApi = CreateAuthenticationApi(session.Context);
        var loginRequest = new LoginRequest
        {
            Email = "nouser@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await authApi.Login(loginRequest, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.TypeOf<NotFoundFailure404>());
    }

    [Test]
    public async Task Login_IncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange
        await using var session = await SqliteTestSession.CreateAsync();
        var authApi = CreateAuthenticationApi(session.Context);
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            Surname = "User"
        };
        await authApi.Register(registerRequest, CancellationToken.None);
        
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await authApi.Login(loginRequest, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.TypeOf<UnauthorizedFailure401>());
    }
}

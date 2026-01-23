using System.Diagnostics;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockyapi.Repository.User;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.models;

namespace stockyapi.Application.Auth;

public class AuthenticationApi : IAuthenticationApi
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthenticationApi(TokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }
    
    public async Task<Result<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        // Verify User Exists
        string email = request.Email;
        string password = request.Password;
        
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user.IsFailure)
        {
            return user.Failure;
        }

        // Verify Password
        if (!BCrypt.Net.BCrypt.Verify(password, user.Value.Password))
            return new UnauthorizedFailure401("Invalid password");
        
        // Create Token
        var token = _tokenService.CreateToken(user.Value);
        return Result<LoginResponse>.Success(new LoginResponse(token));
    }

    public async Task<Result<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        // Variables
        string email = request.Email;
        string password = request.Password;
        string firstName = request.FirstName;
        string surname = request.Surname;
        
        // Verify User DNE
        if (await _userRepository.UserExistsByEmailAsync(email))
        {
            return new ConflictFailure409("Email address already exists.");
        }

        var user = new UserModel
        {
            FirstName = firstName,
            Surname = surname,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Create user first
        await _userRepository.CreateUserAsync(user);

        // Return user with related entities
        var token = _tokenService.CreateToken(user);
        return Result<RegisterResponse>.Success(new RegisterResponse(token, user.Email, user.Id.ToString()));
    }
}
using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Application.Commands.User;
using stockyapi.Middleware;
using stockyapi.Repository.User;
using stockyapi.Services;

namespace stockyapi.Application.Auth;

public class AuthenticationApi : IAuthenticationApi
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthenticationApi(ITokenService tokenService, IUserRepository userRepository)
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
        if (user is null)
            return new NotFoundFailure404("User was not found");

        // Verify Password
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            return new UnauthorizedFailure401("Invalid password");
        
        // Create Token
        var token = _tokenService.CreateToken(user);
        return Result<LoginResponse>.Success(new LoginResponse(token));
    }

    public async Task<Result<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await _userRepository.UserExistsByEmailAsync(request.Email))
            return new ConflictFailure409("Email address already exists.");

        var command = new UserCreateCommand(
            request.FirstName,
            request.Surname,
            request.Email,
            request.Password);

        var user = await _userRepository.CreateUserAsync(command, cancellationToken);

        var token = _tokenService.CreateToken(user);
        return Result<RegisterResponse>.Success(new RegisterResponse(token, user.Email, user.Id.ToString()));
    }
}
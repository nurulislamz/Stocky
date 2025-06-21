using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IAuthService
{
    public Task<LoginResponse> LoginUser(string email, string password);
    public Task<RegisterResponse> CreateNewUser(string firstName, string surname, string email, string password);
}

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public AuthService(ITokenService tokenService, ApplicationDbContext context, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
        _context = context;
    }

    public async Task<LoginResponse> LoginUser(string email, string password)
    {
        // Verify User Exists
        var user = await _userService.VerifyUserExists(email);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Invalid credentials",
            };
        }

        // Verify Password
        if (!_userService.VerifyUserPassword(password, user.Password))
        {
            return new LoginResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Invalid credentials",
            };
        }
        
        // Create Token
        var token = _tokenService.CreateToken(user);

        return new LoginResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new LoginData { Token = token }
        };
    }

    public async Task<RegisterResponse> CreateNewUser(string firstName, string surname, string email, string password)
    {
        // Verify User DNE
        var checkIfUserExists = await _userService.VerifyUserExists(email);
        if (checkIfUserExists != null)
        {
            return new RegisterResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Email address already exists",
            };
        }

        var userCreated = await _userService.CreateUser(firstName, surname, email, password);
        if (userCreated == null)
        {
            return new RegisterResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Failed to Create User",
            };
        }
        
        var token = _tokenService.CreateToken(userCreated);
        return new RegisterResponse
        {
            Success = true,
            StatusCode = 201,
            Data = new RegisterData
            {
                Token = token,
                Email = userCreated.Email,
                UserId = userCreated.Id.ToString()
            }
        };
    }
}
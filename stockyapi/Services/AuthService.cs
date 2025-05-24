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

    public AuthService(ITokenService tokenService, ApplicationDbContext context)
    {
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<LoginResponse> LoginUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return new LoginResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Invalid credentials",
            };

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            return new LoginResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Invalid credentials",
            };
        
        var token = _tokenService.CreateToken(user);
        LoginData loginData = new LoginData
        {
            Token = token
        };

        return new LoginResponse
        {
            Success = true,
            StatusCode = 200,
            Data = loginData
        };
    }

    public async Task<RegisterResponse> CreateNewUser(string firstName, string surname, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return new RegisterResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "Email address already exists",
            };

        var user = new UserModel
        {
            FirstName = firstName,
            Surname = surname,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.User, // Default role
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        RegisterData registerData = new RegisterData
        {
            Token = token
        };

        return new RegisterResponse
        {
            Success = true,
            StatusCode = 201,
            Data = registerData
        };
    }
    
}
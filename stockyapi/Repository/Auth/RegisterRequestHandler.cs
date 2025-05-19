using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Repository.Auth;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public RegisterRequestHandler(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        // Check if user exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return new RegisterResponse { Success = false, Error = "Email already registered" };

        var user = new UserModel
        {
            FirstName = request.FirstName,
            Surname = request.Surname,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _tokenService.CreateToken(user);

        return new RegisterResponse
        {
            Success = true,
            Token = token,
        };
    }
}

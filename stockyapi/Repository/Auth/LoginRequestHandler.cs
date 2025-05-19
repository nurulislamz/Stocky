using MediatR;
using Microsoft.Identity.Client;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stockymodels.Data;

namespace stockyapi.Repository.Auth;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public LoginRequestHandler(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        // Find user
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
            return new LoginResponse { Success = false, Error = "Invalid credentials" };


        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return new LoginResponse { Success = false, Error = "Invalid credentials" };

        // JWT token generation here
        var token = _tokenService.CreateToken(user);

        return new LoginResponse
        {
            Success = true,
            Token = token,
        };
    }
}
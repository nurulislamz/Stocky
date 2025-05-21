using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.Repository.Auth;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly AuthService _authService;

    public LoginRequestHandler(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _authService.ValidateUserCredentials(request.Email, request.Password);
    }
}
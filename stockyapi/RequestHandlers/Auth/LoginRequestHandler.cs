using MediatR;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.Data;

namespace stockyapi.RequestHandlers.Auth;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;

    public LoginRequestHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _authService.LoginUser(request.Email, request.Password);
    }
}
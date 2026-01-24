using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Middleware;

namespace stockyapi.Application.Auth;

public interface IAuthenticationApi
{
    public Task<Result<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken);
    public Task<Result<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken);
}
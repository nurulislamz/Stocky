using System.Diagnostics;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockyapi.Repository.User;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyapi.Services;
using stockymodels.models;

namespace stockyapi.Application.Auth;

public interface IAuthenticationApi
{
    public Task<Result<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken);
    public Task<Result<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken);
}
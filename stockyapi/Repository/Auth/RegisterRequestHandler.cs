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
    private readonly IAuthService _authService;

    public RegisterRequestHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        return await _authService.CreateNewUser(request.FirstName,request.Surname,request.Email, request.Password);
    }
}

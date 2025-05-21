using Microsoft.EntityFrameworkCore;
using stockymodels.models;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IAuthService
{
    public Task<LoginResponse> ValidateUserCredentials(string email, string password);
    public Task<RegisterResponse> CreateNewUser(string firstName, string surname, string email, string password);
}
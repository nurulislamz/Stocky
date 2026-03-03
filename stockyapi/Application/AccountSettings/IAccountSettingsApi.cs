using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Middleware;

namespace stockyapi.Application.Auth;

public interface IAccountSettingsApi
{
    public Task<Result<string>> ChangeName(ChangeNameRequest request, CancellationToken cancellationToken);
}
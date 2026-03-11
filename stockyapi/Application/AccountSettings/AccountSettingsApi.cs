using stockyapi.Application.Auth.Login;
using stockyapi.Application.Auth.Register;
using stockyapi.Application.Commands.User;
using stockyapi.Middleware;
using stockyapi.Repository.User;
using stockyapi.Services;
using stockymodels.Data.Configurations;

namespace stockyapi.Application.Auth;

public class AccountSettingsApi : IAccountSettingsApi
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public AccountSettingsApi(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<string>> ChangeName(ChangeNameRequest request, CancellationToken cancellationToken)
    {

        if (request.FirstName == null && request.Surname == null) {
            return new BadRequestFailure400("both things were null");
        }

        if (_userContext.FirstName == request.FirstName)
        {
            return new ValidationFailure422("");
        }

        if (_userContext.Surname == request.Surname)
        {
            return new ValidationFailure422("");
        }

        var updated = await _userRepository.ChangeName(request.FirstName!, request.Surname!, cancellationToken);
        return Result<string>.Success(updated.FirstName + " " + updated.Surname);
    }
}
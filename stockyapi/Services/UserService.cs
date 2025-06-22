using stockymodels.models;
using stockyapi.Repository.User;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IUserService
{
    public Task<UserModel?> VerifyUserExists(string email);
    public bool VerifyUserPassword(string requestPassword, string userPassword);
    public Task<UserModel> CreateUser(string firstName, string surname, string email, string password);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Authentication methods
    public async Task<UserModel?> VerifyUserExists(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public bool VerifyUserPassword(string requestPassword, string userPassword)
    {
        return BCrypt.Net.BCrypt.Verify(requestPassword, userPassword);
    }

    // User creation with related entities
    public async Task<UserModel> CreateUser(string firstName, string surname, string email, string password)
    {
        var user = new UserModel
        {
            FirstName = firstName,
            Surname = surname,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Create user first
        var createdUser = await _userRepository.CreateAsync(user);

        // Create portfolio using repository
        var portfolio = new PortfolioModel
        {
            Id = createdUser.Id,
            UserId = createdUser.Id,
            TotalValue = 0,
            CashBalance = 0,
            InvestedAmount = 0,
            User = createdUser,
            StockHoldings = new List<StockHoldingModel>(),
            Transactions = new List<TransactionModel>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepository.CreateUserPortfolioAsync(portfolio);

        // Create preferences using repository
        var preferences = new UserPreferencesModel
        {
            Id = createdUser.Id,
            UserId = createdUser.Id,
            Theme = "light",
            Currency = "USD",
            Language = "en",
            EmailNotifications = true,
            PushNotifications = true,
            PriceAlerts = true,
            NewsAlerts = true,
            DefaultCurrency = "USD",
            Timezone = "UTC",
            User = createdUser,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepository.CreateUserPreferencesAsync(preferences);

        // Return user with related entities
        return await _userRepository.GetByIdAsync(createdUser.Id);
    }
}

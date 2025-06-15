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

        // Create user with related entities
        var createdUser = await _userRepository.CreateAsync(user);

        // Create portfolio
        var portfolio = new PortfolioModel
        {
            TotalValue = 0,
            CashBalance = 0,
            InvestedAmount = 0,
            StockHoldings = new List<StockHoldingModel>(),
            Transactions = new List<TransactionModel>(),
            User = createdUser
        };

        // Create preferences
        var preferences = new UserPreferencesModel
        {
            Theme = "light",
            Currency = "USD",
            Language = "en",
            EmailNotifications = true,
            PushNotifications = true,
            PriceAlerts = true,
            NewsAlerts = true,
            DefaultCurrency = "USD",
            Timezone = "UTC",
            User = createdUser
        };

        // Initialize empty collections
        createdUser.Watchlist = new List<WatchlistModel>();
        createdUser.PriceAlerts = new List<PriceAlertModel>();

        // Update user with related entities
        return await _userRepository.UpdateAsync(createdUser);
    }
}

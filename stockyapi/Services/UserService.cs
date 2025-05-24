using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;
using stockyapi.Responses;

namespace stockyapi.Services;

public interface IUserService
{
    public Task<UserModel?> VerifyUserExists(string email);
    public bool VerifyUserPassword(string requestPassword, string userPassword);
    public Task<UserModel> UserCreate(string firstName, string surname, string email, string password);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> VerifyUserExists(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public bool VerifyUserPassword(string requestPassword, string userPassword)
    {
        return BCrypt.Net.BCrypt.Verify(requestPassword, userPassword);
    }

    public async Task<UserModel> UserCreate(string firstName, string surname, string email, string password)
    {
        // Start a transaction
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var user = new UserModel
            {
                FirstName = firstName,
                Surname = surname,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = UserRole.User, // Default role
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var portfolio = new PortfolioModel
            {
                TotalValue = 0,
                CashBalance = 0,
                InvestedAmount = 0,
                StockHoldings = new List<StockHoldingModel>(),
                Transactions = new List<TransactionModel>(),
                User = user
            };

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
                User = user
            };

            // Initialize empty collections
            user.Watchlist = new List<WatchlistModel>();
            user.PriceAlerts = new List<PriceAlertModel>();

            _context.Users.Add(user);
            _context.Portfolios.Add(portfolio);
            _context.UserPreferences.Add(preferences);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using stockyapi.Middleware;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }
    
    public async Task<bool> UserExistsByIdAsync(Guid userId)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == userId);
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task CreateUserAsync(UserModel user)
    {
        user.Portfolio = new PortfolioModel
        {
            Id = user.Id,
            UserId = user.Id,
            TotalValue = 0,
            CashBalance = 0,
            InvestedAmount = 0,
            User = user,
            Funds = new List<FundsTransactionModel>(),
            StockHoldings = new List<StockHoldingModel>(),
            Transactions = new List<AssetTransactionModel>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create preferences using repository
        user.Preferences = new UserPreferencesModel
        {
            Id = user.Id,
            UserId = user.Id,
            Theme = Theme.Light,
            Currency = DefaultCurrency.GDP,
            Language = Language.English,
            EmailNotifications = true,
            PushNotifications = true,
            PriceAlerts = true,
            NewsAlerts = true,
            Timezone = "UTC",
            User = user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(UserModel user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}

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

    public async Task<Result<UserModel>> GetUserByIdAsync(Guid id)
    {
        var user =  await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id);

        return user != null
            ? Result<UserModel>.Success(user)
            : new NotFoundFailure404("User not found");
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

    public async Task<Result<UserModel>> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == email);
        
        return user == null ? Result<UserModel>.Fail(new NotFoundFailure404("User not found")) : Result<UserModel>.Success(user);
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

    public async Task<Result<UserModel>> UpdateUserAsync(UserModel user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Result<UserModel>.Success(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await GetUserByIdAsync(id);
        if (user.IsFailure)
        {
            throw new DbUpdateException("Failed to delete user");
        }
        
        _context.Users.Remove(user.Value);
        await _context.SaveChangesAsync();
    }

    // public async Task<bool> UpdateLastLoginAsync(Guid userId)
    // {
    //     var user = await GetByIdAsync(userId);
    //     if (user == null) return false;
    //
    //     user.LastLogin = DateTime.UtcNow;
    //     await _context.SaveChangesAsync();
    //     return true;
    // }
    //
    // public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
    // {
    //     var user = await GetByIdAsync(userId);
    //     if (user == null) return false;
    //
    //     user.IsActive = isActive;
    //     await _context.SaveChangesAsync();
    //     return true;
    // }
    //
    // public async Task<bool> UpdateUserRoleAsync(Guid userId, UserRole role)
    // {
    //     var user = await GetByIdAsync(userId);
    //     if (user == null) return false;
    //
    //     user.Role = role;
    //     await _context.SaveChangesAsync();
    //     return true;
    // }
    //
    // public async Task<UserPreferencesModel?> GetUserPreferencesAsync(Guid userId)
    // {
    //     return await _context.UserPreferences
    //         .FirstOrDefaultAsync(p => p.UserId == userId);
    // }
    //
    // public async Task<UserPreferencesModel> CreateUserPreferencesAsync(UserPreferencesModel preferences)
    // {
    //     await _context.UserPreferences.AddAsync(preferences);
    //     await _context.SaveChangesAsync();
    //     return preferences;
    // }
    //
    // public async Task<UserPreferencesModel> UpdateUserPreferencesAsync(UserPreferencesModel preferences)
    // {
    //     _context.UserPreferences.Update(preferences);
    //     await _context.SaveChangesAsync();
    //     return preferences;
    // }
    //
    // public async Task<PortfolioModel?> GetUserPortfolioAsync(Guid userId)
    // {
    //     return await _context.Portfolios
    //         .Include(p => p.StockHoldings)
    //         .Include(p => p.Transactions)
    //         .FirstOrDefaultAsync(p => p.Id == userId);
    // }
    //
    // public async Task<PortfolioModel> CreateUserPortfolioAsync(PortfolioModel portfolio)
    // {
    //     await _context.Portfolios.AddAsync(portfolio);
    //     await _context.SaveChangesAsync();
    //     return portfolio;
    // }
    //
    // public async Task<PortfolioModel> UpdateUserPortfolioAsync(PortfolioModel portfolio)
    // {
    //     _context.Portfolios.Update(portfolio);
    //     await _context.SaveChangesAsync();
    //     return portfolio;
    // }
    //
    // public async Task<IEnumerable<WatchlistModel>> GetUserWatchlistAsync(Guid userId)
    // {
    //     return await _context.Watchlist
    //         .Where(w => w.Id == userId)
    //         .ToListAsync();
    // }
    //
    // public async Task<WatchlistModel> AddToWatchlistAsync(Guid userId, string ticker)
    // {
    //     var watchlistItem = new WatchlistModel
    //     {
    //         Id = userId,
    //         Symbol = ticker,
    //         AddedAt = DateTime.UtcNow
    //     };
    //
    //     await _context.Watchlist.AddAsync(watchlistItem);
    //     await _context.SaveChangesAsync();
    //     return watchlistItem;
    // }
    //
    // public async Task RemoveFromWatchlistAsync(Guid userId, string ticker)
    // {
    //     var watchlistItem = await _context.Watchlist
    //         .FirstOrDefaultAsync(w => w.Id == userId && w.Symbol == ticker);
    //
    //     if (watchlistItem != null)
    //     {
    //         _context.Watchlist.Remove(watchlistItem);
    //         await _context.SaveChangesAsync();
    //     }
    // }
    //
    // public async Task<IEnumerable<PriceAlertModel>> GetUserPriceAlertsAsync(Guid userId)
    // {
    //     return await _context.PriceAlerts
    //         .Where(p => p.Id == userId)
    //         .ToListAsync();
    // }
    //
    // public async Task<PriceAlertModel> AddPriceAlertAsync(Guid userId, string ticker, decimal targetPrice)
    // {
    //     var priceAlert = new PriceAlertModel
    //     {
    //         Id = userId,
    //         Symbol = ticker,
    //         TargetPrice = targetPrice,
    //         CreatedAt = DateTime.UtcNow,
    //         IsTriggered = true
    //     };
    //
    //     await _context.PriceAlerts.AddAsync(priceAlert);
    //     await _context.SaveChangesAsync();
    //     return priceAlert;
    // }
    //
    // public async Task RemovePriceAlertAsync(Guid userId, string ticker)
    // {
    //     var priceAlert = await _context.PriceAlerts
    //         .FirstOrDefaultAsync(p => p.Id == userId && p.Symbol == ticker);
    //
    //     if (priceAlert != null)
    //     {
    //         _context.PriceAlerts.Remove(priceAlert);
    //         await _context.SaveChangesAsync();
    //     }
    // }
}

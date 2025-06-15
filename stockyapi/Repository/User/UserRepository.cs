using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserModel?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Preferences)
            .Include(u => u.Portfolio)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<UserModel?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Preferences)
            .Include(u => u.Portfolio)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<UserModel>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Preferences)
            .Include(u => u.Portfolio)
            .ToListAsync();
    }

    public async Task<UserModel> CreateAsync(UserModel user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<UserModel> UpdateAsync(UserModel user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UpdateLastLoginAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUserRoleAsync(Guid userId, UserRole role)
    {
        var user = await GetByIdAsync(userId);
        if (user == null) return false;

        user.Role = role;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserPreferencesModel?> GetUserPreferencesAsync(Guid userId)
    {
        return await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserPreferencesModel> UpdateUserPreferencesAsync(UserPreferencesModel preferences)
    {
        _context.UserPreferences.Update(preferences);
        await _context.SaveChangesAsync();
        return preferences;
    }

    public async Task<PortfolioModel?> GetUserPortfolioAsync(Guid userId)
    {
        return await _context.Portfolios
            .Include(p => p.StockHoldings)
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.Id == userId);
    }

    public async Task<IEnumerable<WatchlistModel>> GetUserWatchlistAsync(Guid userId)
    {
        return await _context.Watchlist
            .Where(w => w.Id == userId)
            .ToListAsync();
    }

    public async Task<WatchlistModel> AddToWatchlistAsync(Guid userId, string ticker)
    {
        var watchlistItem = new WatchlistModel
        {
            Id = userId,
            Symbol = ticker,
            AddedAt = DateTime.UtcNow
        };

        await _context.Watchlist.AddAsync(watchlistItem);
        await _context.SaveChangesAsync();
        return watchlistItem;
    }

    public async Task RemoveFromWatchlistAsync(Guid userId, string ticker)
    {
        var watchlistItem = await _context.Watchlist
            .FirstOrDefaultAsync(w => w.Id == userId && w.Symbol == ticker);

        if (watchlistItem != null)
        {
            _context.Watchlist.Remove(watchlistItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PriceAlertModel>> GetUserPriceAlertsAsync(Guid userId)
    {
        return await _context.PriceAlerts
            .Where(p => p.Id == userId)
            .ToListAsync();
    }

    public async Task<PriceAlertModel> AddPriceAlertAsync(Guid userId, string ticker, decimal targetPrice)
    {
        var priceAlert = new PriceAlertModel
        {
            Id = userId,
            Symbol = ticker,
            TargetPrice = targetPrice,
            CreatedAt = DateTime.UtcNow,
            IsTriggered = true
        };

        await _context.PriceAlerts.AddAsync(priceAlert);
        await _context.SaveChangesAsync();
        return priceAlert;
    }

    public async Task RemovePriceAlertAsync(Guid userId, string ticker)
    {
        var priceAlert = await _context.PriceAlerts
            .FirstOrDefaultAsync(p => p.Id == userId && p.Symbol == ticker);

        if (priceAlert != null)
        {
            _context.PriceAlerts.Remove(priceAlert);
            await _context.SaveChangesAsync();
        }
    }
}

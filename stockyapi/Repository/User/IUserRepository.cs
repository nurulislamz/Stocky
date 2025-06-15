using stockymodels.models;

namespace stockyapi.Repository.User;

public interface IUserRepository
{
    // Basic CRUD operations
    Task<UserModel?> GetByIdAsync(Guid id);
    Task<UserModel?> GetByEmailAsync(string email);
    Task<IEnumerable<UserModel>> GetAllAsync();
    Task<UserModel> CreateAsync(UserModel user);
    Task<UserModel> UpdateAsync(UserModel user);
    Task DeleteAsync(Guid id);

    // User-specific operations
    Task<bool> UpdateLastLoginAsync(Guid userId);
    Task<bool> UpdateUserStatusAsync(Guid userId, bool isActive);
    Task<bool> UpdateUserRoleAsync(Guid userId, UserRole role);
    
    // User preferences operations
    Task<UserPreferencesModel?> GetUserPreferencesAsync(Guid userId);
    Task<UserPreferencesModel> UpdateUserPreferencesAsync(UserPreferencesModel preferences);
    
    // User portfolio operations
    Task<PortfolioModel?> GetUserPortfolioAsync(Guid userId);
    
    // User watchlist operations
    Task<IEnumerable<WatchlistModel>> GetUserWatchlistAsync(Guid userId);
    Task<WatchlistModel> AddToWatchlistAsync(Guid userId, string ticker);
    Task RemoveFromWatchlistAsync(Guid userId, string ticker);
    
    // User price alerts operations
    Task<IEnumerable<PriceAlertModel>> GetUserPriceAlertsAsync(Guid userId);
    Task<PriceAlertModel> AddPriceAlertAsync(Guid userId, string ticker, decimal targetPrice);
    Task RemovePriceAlertAsync(Guid userId, string ticker);
}

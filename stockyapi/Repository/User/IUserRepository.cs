using stockyapi.Middleware;
using stockymodels.models;

namespace stockyapi.Repository.User;

public interface IUserRepository
{
    // Basic CRUD operations
    Task<Result<UserModel>> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<Result<UserModel>> GetUserByEmailAsync(string email);
    Task CreateUserAsync(UserModel user);
    Task<Result<UserModel>> UpdateUserAsync(UserModel user);
    Task DeleteUserAsync(Guid id);

    // // User preferences operations
    // Task<UserPreferencesModel?> GetUserPreferencesAsync(Guid userId);
    // Task<UserPreferencesModel> CreateUserPreferencesAsync(UserPreferencesModel preferences);
    // Task<UserPreferencesModel> UpdateUserPreferencesAsync(UserPreferencesModel preferences);
    //
    // // User watchlist operations
    // Task<IEnumerable<WatchlistModel>> GetUserWatchlistAsync(Guid userId);
    // Task<WatchlistModel> AddToWatchlistAsync(Guid userId, string ticker);
    // Task RemoveFromWatchlistAsync(Guid userId, string ticker);
    //
    // // User price alerts operations
    // Task<IEnumerable<PriceAlertModel>> GetUserPriceAlertsAsync(Guid userId);
    // Task<PriceAlertModel> AddPriceAlertAsync(Guid userId, string ticker, decimal targetPrice);
    // Task RemovePriceAlertAsync(Guid userId, string ticker);
}

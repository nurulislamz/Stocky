using stockyapi.Application.Commands.User;
using stockyapi.Middleware;
using stockymodels.models;

namespace stockyapi.Repository.User;

public interface IUserRepository
{
    // Basic CRUD operations
    Task<UserModel?> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<UserModel?> GetUserByEmailAsync(string email);
    /// <summary>Creates a user from the command and appends a UserCreate event to the event store.</summary>
    Task<UserModel> CreateUserAsync(UserCreateCommand command, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(UserModel user);

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

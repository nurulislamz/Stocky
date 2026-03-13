using stockyapi.Application.Commands.Portfolio;
using stockyapi.Application.Commands.User;
using stockymodels.models;

namespace stockyapi.Repository.User;

public interface IUserRepository
{
    Task<UserModel?> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<UserModel?> GetUserByEmailAsync(string email);

    Task<UserModel> CreateUserAsync(UserCreateCommand userCreateCommand, PortfolioCreationCommand? portfolioCreateCommand = null, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(UserModel user);

    Task<UserModel> ChangeName(string firstName, string surName, CancellationToken cancellationToken = default);

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

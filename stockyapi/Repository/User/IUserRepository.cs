using stockyapi.Application.Commands.Portfolio;
using stockyapi.Application.Commands.User;
using stockymodels.models;

namespace stockyapi.Repository.User;

public interface IUserRepository
{
    Task<UserAggregate?> GetUserByIdAsync(Guid id);
    Task<bool> UserExistsByEmailAsync(string email);
    Task<UserAggregate?> GetUserByEmailAsync(string email);

    Task<UserAggregate> CreateUserAsync(UserCreateCommand userCreateCommand, PortfolioCreationCommand? portfolioCreateCommand = null, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(UserAggregate user);

    Task<UserAggregate> ChangeName(string firstName, string surName, CancellationToken cancellationToken = default);

    // // User preferences operations
    // Task<UserPreferencesAggregate?> GetUserPreferencesAsync(Guid userId);
    // Task<UserPreferencesAggregate> CreateUserPreferencesAsync(UserPreferencesAggregate preferences);
    // Task<UserPreferencesAggregate> UpdateUserPreferencesAsync(UserPreferencesAggregate preferences);
    //
    // // User watchlist operations
    // Task<IEnumerable<WatchlistAggregate>> GetUserWatchlistAsync(Guid userId);
    // Task<WatchlistAggregate> AddToWatchlistAsync(Guid userId, string ticker);
    // Task RemoveFromWatchlistAsync(Guid userId, string ticker);
    //
    // // User price alerts operations
    // Task<IEnumerable<PriceAlertAggregate>> GetUserPriceAlertsAsync(Guid userId);
    // Task<PriceAlertAggregate> AddPriceAlertAsync(Guid userId, string ticker, decimal targetPrice);
    // Task RemovePriceAlertAsync(Guid userId, string ticker);
}

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using stockyapi.Application.Commands.Portfolio;
using stockyapi.Application.Commands.User;
using stockyapi.Repository.Event;
using stockyapi.Middleware;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockyapi.Repository.User;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ApplicationDbContext context,
        IEventRepository eventRepository,
        ILogger<UserRepository> logger)
    {
        _context = context;
        _eventRepository = eventRepository;
        _logger = logger;
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

    public async Task<UserModel> CreateUserAsync(UserCreateCommand userCreateCommand, PortfolioCreationCommand portfolioCreateCommand, CancellationToken cancellationToken = default)
    {
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userCreateCommand.Password);
        var now = DateTimeOffset.UtcNow;
        var validTo = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        var payload = new
        {
            userCreateCommand.FirstName,
            userCreateCommand.Surname,
            userCreateCommand.Email,
            PasswordHash = hashedPassword
        };
        var eventPayloadJson = JsonSerializer.Serialize(payload);

        var userCreationEvent = new EventModel
        {
            Id = Guid.NewGuid(),
            AggregateType = AggregateType.UserId,
            AggregateId = userId,
            SequenceId = 1,
            EventType = EventType.UserCreate,
            EventPayloadJson = eventPayloadJson,
            EventPayloadProtobuf = Array.Empty<byte>(),
            TtStart = now,
            TtEnd = now,
            ValidFrom = now,
            ValidTo = validTo
        };

        var user = new UserModel
        {
            Id = userId,
            FirstName = userCreateCommand.FirstName,
            Surname = userCreateCommand.Surname,
            Email = userCreateCommand.Email,
            Password = hashedPassword,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var portfolio = new PortfolioModel
        {
            Id = portfolioCreateCommand.PortfolioId,
            UserId = portfolioCreateCommand.UserId,
            TotalValue = portfolioCreateCommand.TotalValue,
            CashBalance = portfolioCreateCommand.CashBalance,
            InvestedAmount = portfolioCreateCommand.InvestedAmount
        };

        var preferenceCommand = new UserPreferenceCreationCommand(userId);
        var preferences = new UserPreferencesModel
        {
            Id = preferenceCommand.UserId,
            UserId = preferenceCommand.UserId,
            Theme = preferenceCommand.Theme,
            Currency = preferenceCommand.Currency,
            Language = preferenceCommand.Language,
            EmailNotifications = preferenceCommand.EmailNotifications,
            PushNotifications = preferenceCommand.PushNotifications,
            PriceAlerts = preferenceCommand.PriceAlerts,
            NewsAlerts = preferenceCommand.NewsAlerts,
            Timezone = preferenceCommand.Timezone
        };

        // Single transaction: event + user + portfolio + preferences. If any part fails, nothing is committed.
        _eventRepository.Add(userCreateCommand);
        _context.Users.Add(user);
        _context.Portfolios.Add(portfolio);
        _context.UserPreferences.Add(preferences);
        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        // TODO: Need to modify this so it doesn't update everything, make it so it updates a specific parameter like email or password
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(UserModel user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}

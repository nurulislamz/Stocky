using System.Text.Json;
using Microsoft.EntityFrameworkCore;
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

    public async Task<UserModel> CreateUserAsync(UserCreateCommand command, CancellationToken cancellationToken = default)
    {
        var userId = Guid.NewGuid();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var now = DateTimeOffset.UtcNow;
        var validTo = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);

        var payload = new
        {
            command.FirstName,
            command.Surname,
            command.Email,
            PasswordHash = hashedPassword
        };
        var eventPayloadJson = JsonSerializer.Serialize(payload);

        var evt = new EventModel
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
            FirstName = command.FirstName,
            Surname = command.Surname,
            Email = command.Email,
            Password = hashedPassword,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var portfolio = new PortfolioModel
        {
            Id = userId,
            UserId = userId,
            TotalValue = 0,
            CashBalance = 0,
            InvestedAmount = 0
        };

        var preferences = new UserPreferencesModel
        {
            Id = userId,
            UserId = userId,
            Theme = Theme.Light,
            Currency = DefaultCurrency.GDP,
            Language = Language.English,
            EmailNotifications = true,
            PushNotifications = true,
            PriceAlerts = true,
            NewsAlerts = true,
            Timezone = "UTC"
        };

        // Single transaction: event + user + portfolio + preferences. If any part fails, nothing is committed.
        _eventRepository.Add(evt);
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

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;
using stockytests.Integration;

namespace stockytests.Helpers;

internal sealed class SqliteTestSession : IAsyncDisposable
{
    public ApplicationDbContext Context { get; }
    public Guid UserId { get; private set; }
    public string UserEmail { get; private set; }
    public Guid PortfolioId { get; private set; }
    private readonly SqliteConnection _connection;

    private SqliteTestSession(ApplicationDbContext context, SqliteConnection connection)
    {
        Context = context;
        _connection = connection;
    }

    public static async Task<SqliteTestSession> CreateAsync()
    {
        SqliteConnection connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return new SqliteTestSession(context, connection);
    }

    public async Task SetupUser(decimal initialCashBalance = 0m, decimal initialInvestedAmount = 0m)
    {
        UserId = Guid.NewGuid();
        PortfolioId = Guid.NewGuid();
        UserEmail = $"integration+{Guid.NewGuid():N}@example.com";

        await Context.Users.AddAsync(new UserModel
        {
            Id = UserId,
            FirstName = "Integration",
            Surname = "Tester",
            Email = UserEmail,
            Password = "IntegrationPassword1!",
            Role = UserRole.User
        });

        await Context.Portfolios.AddAsync(new PortfolioModel
        {
            Id = PortfolioId,
            UserId = UserId,
            CashBalance = initialCashBalance,
            TotalValue = initialCashBalance + initialInvestedAmount,
            InvestedAmount = initialInvestedAmount
        });

        await Context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

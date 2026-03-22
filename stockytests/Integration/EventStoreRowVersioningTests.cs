using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using stockymodels.EventStore;
using stockymodels.Events;
using stockymodels.Events.Funds;
using Testcontainers.PostgreSql;

namespace stockytests.Integration;

[TestFixture]
[Category("Integration")]
public class EventStoreRowVersioningTests
{
	private PostgreSqlContainer _container = null!;
	private string _connectionString = null!;

	[OneTimeSetUp]
	public async Task SetUpContainer()
	{
		_container = new PostgreSqlBuilder()
			.WithImage("postgres:16-alpine")
			.Build();
		await _container.StartAsync();
		_connectionString = _container.GetConnectionString();

		await ApplySchemaAsync();
	}

	[OneTimeTearDown]
	public async Task TearDownContainer()
	{
		await _container.DisposeAsync();
	}

	private async Task ApplySchemaAsync()
	{
		var assembly = typeof(PostgresEventStore).Assembly;
		var resourceName = "stockymodels.Migrations.001CreateEventStore.sql";
		await using var stream = assembly.GetManifestResourceStream(resourceName)
			?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
		using var reader = new StreamReader(stream);
		var sql = await reader.ReadToEndAsync();

		await using var conn = new NpgsqlConnection(_connectionString);
		await conn.OpenAsync();
		await using var cmd = new NpgsqlCommand(sql, conn);
		await cmd.ExecuteNonQueryAsync();

		// Disable RLS for tests (avoids needing app.user_id session variable)
		await using var disableRls = new NpgsqlCommand(
			"ALTER TABLE stockydb.\"Events\" DISABLE ROW LEVEL SECURITY;", conn);
		await disableRls.ExecuteNonQueryAsync();
	}

	[Test]
	public async Task RowVersioning_AppendWithCorrectVersion_Succeeds()
	{
		var userId = Guid.NewGuid();
		var fundId = Guid.NewGuid();
		var context = new AppendContext(userId, Guid.NewGuid());

		await using var store = new PostgresEventStore(
			_connectionString,
			NullLogger<PostgresEventStore>.Instance,
			concurrencyLevel: ConcurrencyLevel.RowVersioning);

		// New stream: version 0
		var version = await store.GetStreamVersionAsync("FundId", fundId);
		Assert.That(version, Is.EqualTo(0));

		var evt = new StockyEvent("FundId", fundId, new FundsDepositedStockyEvent
		{
			Amount = 100m,
			CashBalanceBefore = 0m,
			CashBalanceAfter = 100m,
			PortfolioTotalValueBefore = 0m,
			PortfolioTotalValueAfter = 100m,
			OccurredAt = DateTimeOffset.UtcNow,
			RequestId = Guid.NewGuid()
		});
		var cmd = new stockyapi.Application.Commands.Funds.DepositFundsCommand(100m);

		var (_, inserted) = await store.RegisterEventAsync(cmd, evt, context, expectedVersion: 0);
		Assert.That(inserted.AggregateSequenceId, Is.EqualTo(1));

		// Version is now 1
		version = await store.GetStreamVersionAsync("FundId", fundId);
		Assert.That(version, Is.EqualTo(1));

		// Append second event with expectedVersion 1
		var evt2 = new StockyEvent("FundId", fundId, new FundsDepositedStockyEvent
		{
			Amount = 50m,
			CashBalanceBefore = 100m,
			CashBalanceAfter = 150m,
			PortfolioTotalValueBefore = 100m,
			PortfolioTotalValueAfter = 150m,
			OccurredAt = DateTimeOffset.UtcNow,
			RequestId = Guid.NewGuid()
		});
		var cmd2 = new stockyapi.Application.Commands.Funds.DepositFundsCommand(50m);

		var (_, inserted2) = await store.RegisterEventAsync(cmd2, evt2, context, expectedVersion: 1);
		Assert.That(inserted2.AggregateSequenceId, Is.EqualTo(2));
	}

	[Test]
	public void RowVersioning_AppendWithStaleVersion_Throws()
	{
		var userId = Guid.NewGuid();
		var fundId = Guid.NewGuid();
		var context = new AppendContext(userId, Guid.NewGuid());

		// Use a single store instance to append two events
		using (var store = new PostgresEventStore(
			_connectionString,
			NullLogger<PostgresEventStore>.Instance,
			concurrencyLevel: ConcurrencyLevel.RowVersioning))
		{
			var evt = new StockyEvent("FundId", fundId, new FundsDepositedStockyEvent
			{
				Amount = 100m,
				CashBalanceBefore = 0m,
				CashBalanceAfter = 100m,
				PortfolioTotalValueBefore = 0m,
				PortfolioTotalValueAfter = 100m,
				OccurredAt = DateTimeOffset.UtcNow,
				RequestId = Guid.NewGuid()
			});
			var cmd = new stockyapi.Application.Commands.Funds.DepositFundsCommand(100m);
			store.RegisterEventAsync(cmd, evt, context, expectedVersion: 0).GetAwaiter().GetResult();
		}

		// Second append with stale expectedVersion 0 (actual is 1)
		using var store2 = new PostgresEventStore(
			_connectionString,
			NullLogger<PostgresEventStore>.Instance,
			concurrencyLevel: ConcurrencyLevel.RowVersioning);

		var evt2 = new StockyEvent("FundId", fundId, new FundsDepositedStockyEvent
		{
			Amount = 50m,
			CashBalanceBefore = 100m,
			CashBalanceAfter = 150m,
			PortfolioTotalValueBefore = 100m,
			PortfolioTotalValueAfter = 150m,
			OccurredAt = DateTimeOffset.UtcNow,
			RequestId = Guid.NewGuid()
		});
		var cmd2 = new stockyapi.Application.Commands.Funds.DepositFundsCommand(50m);

		var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
			await store2.RegisterEventAsync(cmd2, evt2, context, expectedVersion: 0));

		Assert.That(ex!.Message, Does.Contain("Stream version conflict"));
		Assert.That(ex.Message, Does.Contain("expected 0"));
		Assert.That(ex.Message, Does.Contain("actual 1"));
	}

	[Test]
	public void RowVersioning_WithoutExpectedVersion_Throws()
	{
		using var store = new PostgresEventStore(
			_connectionString,
			NullLogger<PostgresEventStore>.Instance,
			concurrencyLevel: ConcurrencyLevel.RowVersioning);

		var evt = new StockyEvent("FundId", Guid.NewGuid(), new FundsDepositedStockyEvent
		{
			Amount = 100m,
			CashBalanceBefore = 0m,
			CashBalanceAfter = 100m,
			PortfolioTotalValueBefore = 0m,
			PortfolioTotalValueAfter = 100m,
			OccurredAt = DateTimeOffset.UtcNow,
			RequestId = Guid.NewGuid()
		});
		var cmd = new stockyapi.Application.Commands.Funds.DepositFundsCommand(100m);
		var context = new AppendContext(Guid.NewGuid(), Guid.NewGuid());

		var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
			await store.RegisterEventAsync(cmd, evt, context, expectedVersion: null));

		Assert.That(ex!.Message, Does.Contain("expectedVersion"));
		Assert.That(ex.Message, Does.Contain("RowVersioning"));
	}
}

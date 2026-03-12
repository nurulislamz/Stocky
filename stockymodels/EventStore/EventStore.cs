using Npgsql;
using Dapper;
using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.Data;

public class PostgresEventStore : IDisposable, IAsyncDisposable
{
	private readonly string _connectionString;
	public readonly NpgsqlConnection Connection;

	public PostgresEventStore(string connectionString)
	{
		_connectionString = connectionString;
		Connection = new NpgsqlConnection(_connectionString);
		Connection.Open();
	}

	public async void InsertEvent(StockyEventModel)
	{
		await Connection.ExecuteAsync("""I )
	}

	public void Dispose()
	{
		Connection.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		await Connection.DisposeAsync();
	}
}
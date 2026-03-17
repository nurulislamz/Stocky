using Npgsql;

namespace stockymodels.EventStore;

/// <summary>
/// Scoped factory: one connection per scope (e.g. per request). Register as Scoped in DI.
/// Disposes the connection when the scope is disposed.
/// </summary>
public sealed class NpgsqlConnectionFactory : IDbConnectionFactory, IDisposable, IAsyncDisposable
{
	private readonly string _connectionString;
	private NpgsqlConnection? _connection;
	private readonly SemaphoreSlim _lock = new(1, 1);

	public NpgsqlConnectionFactory(string connectionString)
	{
		_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
	}

	public async ValueTask<NpgsqlConnection> GetConnectionAsync(CancellationToken ct = default)
	{
		if (_connection != null)
			return _connection;

		await _lock.WaitAsync(ct);
		try
		{
			if (_connection != null)
				return _connection;
			_connection = new NpgsqlConnection(_connectionString);
			await _connection.OpenAsync(ct);
			return _connection;
		}
		finally
		{
			_lock.Release();
		}
	}

	public void Dispose() => _connection?.Dispose();

	public async ValueTask DisposeAsync() => await (_connection?.DisposeAsync() ?? ValueTask.CompletedTask);
}

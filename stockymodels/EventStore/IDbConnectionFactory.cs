using Npgsql;

namespace stockymodels.EventStore;

/// <summary>
/// Provides a database connection for the current scope. When registered as Scoped in DI,
/// returns the same connection for the scope lifetime; the scope (container) owns and disposes it.
/// Callers must not dispose the returned connection.
/// </summary>
public interface IDbConnectionFactory
{
	/// <summary>Get the connection for the current scope. Do not dispose the returned connection.</summary>
	ValueTask<NpgsqlConnection> GetConnectionAsync(CancellationToken ct = default);
}

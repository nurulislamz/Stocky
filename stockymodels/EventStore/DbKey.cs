namespace stockymodels.EventStore;

/// <summary>
/// Keyed DI service keys for distinguishing read vs write NpgsqlDataSource/NpgsqlConnection.
/// </summary>
public enum DbKey
{
	Write,
	Read
}

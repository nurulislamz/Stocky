namespace stockymodels.Events;

/// <summary>
/// Event payload that applies to a given model type. Used by EventStreamBuilder to replay events.
/// </summary>
public interface IEventApplier<in TModel> where TModel : class
{
	void Apply(TModel model);
}

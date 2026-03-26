namespace stockymodels.EventStore;

public enum ConcurrencyType
{
	OptimisticConcurrency,
	LockOnAggregate,
	RowVersion
}

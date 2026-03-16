using Microsoft.Extensions.Logging;
using stockymodels.Events.Funds;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockymodels.EventStore;

/// <summary>
/// Replays the fund stream (AggregateType FundId, AggregateId = FundAccountId) to build FundAccountAggregate.
/// No portfolio or holding logic; only fund events apply.
/// </summary>
public class FundAccountBuilder : EventStreamBuilder<FundAccountEvent, FundAccountAggregate, FundAccountCreatedStockyEvent>
{
	public FundAccountBuilder(PostgresEventStoreReader eventStore, ILogger<PostgresEventStore> logger)
		: base(eventStore, logger)
	{
	}

	protected override int AggregateTypeId => (int)AggregateType.FundId;

	protected override FundAccountAggregate CreateFrom(FundAccountCreatedStockyEvent created, Guid fundAccountId, Guid tradingAccountId)
	{
		var occurred = created.OccurredAt.UtcDateTime;
		return new FundAccountAggregate
		{
			Id = fundAccountId,
			TradingAccountId = tradingAccountId,
			CashBalance = created.InitialCashBalance,
			CreatedAt = occurred,
			UpdatedAt = occurred
		};
	}
}

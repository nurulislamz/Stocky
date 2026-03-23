using Microsoft.Extensions.Logging;
using stockymodels.Events.Portfolios;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockymodels.EventStore;

public class PortfolioBuilder : EventStreamBuilder<PortfolioEvent, PortfolioAggregate, PortfolioCreatedStockyEvent>
{
	public PortfolioBuilder(IEventStoreReader eventStoreReader, ILogger<PostgresEventStore> logger)
		: base(eventStoreReader, logger)
	{
	}

	protected override int AggregateTypeId => (int)AggregateType.PortfolioId;

	protected override PortfolioAggregate CreateFrom(PortfolioCreatedStockyEvent created, Guid portfolioId, Guid tradingAccountId)
	{
		var occurred = created.OccurredAt.UtcDateTime;
		return new PortfolioAggregate
		{
			Id = portfolioId,
			UserId = tradingAccountId,
			TotalValue = created.TotalValue,
			CashBalance = created.CashBalance,
			InvestedAmount = created.InvestedAmount,
			CreatedAt = occurred,
			UpdatedAt = occurred,
			StockHoldings = new List<StockHoldingAggregate>()
		};
	}
}

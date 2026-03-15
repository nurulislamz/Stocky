using Microsoft.Extensions.Logging;
using stockymodels.Events;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockymodels.EventStore;

public class PortfolioBuilder
{
	private readonly PostgresEventStore _eventStore;
	private readonly ILogger<PostgresEventStore> _logger;

	public PortfolioBuilder(PostgresEventStore eventStore, ILogger<PostgresEventStore> logger)
	{
		_eventStore = eventStore;
		_logger = logger;
	}

	public async Task<PortfolioModel?> BuildAsync(Guid portfolioId, Guid userId, CancellationToken ct = default)
	{
		var portfolioEvents = await _eventStore.QueryAllAggregatedEventsAsync<PortfolioEvent>((int)AggregateType.PortfolioId, portfolioId, ct);
		if (portfolioEvents is null or { Length: 0 })
			return null;
		return ApplyEvents(portfolioId, userId, portfolioEvents);
	}

	public PortfolioModel ApplyEvents(Guid portfolioId, Guid userId, PortfolioEvent[] portfolioEvents)
	{
		if (portfolioEvents.Length == 0)
			throw new InvalidOperationException("At least one event is required.");
		if (portfolioEvents[0] is not PortfolioCreatedStockyEvent created)
			throw new InvalidOperationException("First event must be PortfolioCreatedStockyEvent.");

		var portfolio = Create(created, portfolioId, userId);

		foreach (var @event in portfolioEvents.Skip(1))
		{
			if (@event is PortfolioCreatedStockyEvent)
				throw new InvalidOperationException("Cannot have multiple PortfolioCreatedStockyEvent events, please contact support");
			@event.Apply(portfolio);
		}

		return portfolio;
	}

	/// <summary>
	/// Initializes portfolio state from the creation event (Marten-style convention).
	/// </summary>
	public static PortfolioModel Create(PortfolioCreatedStockyEvent created, Guid portfolioId, Guid userId)
	{
		var occurred = created.OccurredAt.UtcDateTime;
		return new PortfolioModel
		{
			Id = portfolioId,
			UserId = userId,
			CashBalance = created.CashBalance,
			TotalValue = created.TotalValue,
			InvestedAmount = created.InvestedAmount,
			CreatedAt = occurred,
			UpdatedAt = occurred,
			StockHoldings = new List<StockHoldingModel>()
		};
	}
}

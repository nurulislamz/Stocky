using stockymodels.Events;
using stockymodels.models;

namespace stockymodels.Events.Portfolios;

public abstract record PortfolioEvent : StockyEventPayload, IEventApplier<PortfolioAggregate>
{
	/// <summary>
	/// Applies this event to the portfolio (polymorphic dispatch; no switch required).
	/// </summary>
	public abstract void Apply(PortfolioAggregate portfolio);
}

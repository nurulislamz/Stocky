using stockymodels.models;

namespace stockymodels.Events;

public abstract record PortfolioEvent : StockyEventPayload
{
	/// <summary>
	/// Applies this event to the portfolio (polymorphic dispatch; no switch required).
	/// </summary>
	public abstract void Apply(PortfolioModel portfolio);
}

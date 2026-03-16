using Microsoft.Extensions.Logging;
using stockymodels.Events.Users;
using stockymodels.models;
using stockymodels.Models.Enums;

namespace stockymodels.EventStore;

/// <summary>
/// Replays the user stream (AggregateType UserId, AggregateId = UserId) to build UserAggregate.
/// </summary>
public class UserBuilder : EventStreamBuilder<UserEvent, UserAggregate, UserCreatedStockyEvent>
{
	public UserBuilder(PostgresEventStoreReader eventStore, ILogger<PostgresEventStore> logger)
		: base(eventStore, logger)
	{
	}

	protected override int AggregateTypeId => (int)AggregateType.UserId;

	protected override UserAggregate CreateFrom(UserCreatedStockyEvent created, Guid userId, Guid tradingAccountId)
	{
		var occurred = created.OccurredAt.UtcDateTime;
		return new UserAggregate
		{
			Id = userId,
			FirstName = created.FirstName,
			Surname = created.Surname,
			Email = created.Email,
			Password = "", // Not stored in events; set by command/write path
			Role = UserRole.User,
			IsActive = true,
			CreatedAt = occurred,
			UpdatedAt = occurred,
			Watchlist = new List<WatchlistAggregate>(),
			PriceAlerts = new List<PriceAlertAggregate>()
		};
	}
}

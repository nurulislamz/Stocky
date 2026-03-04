namespace stockyapi.Application.Commands.Watchlist;

/// <summary>Command for RemoveFromWatchlist event. UserId is set by the handler from context.</summary>
public record RemoveFromWatchlistCommand(string Symbol);

namespace stockyapi.Application.Commands.Watchlist;

/// <summary>Command for AddToWatchlist event. UserId is set by the handler from context.</summary>
public record AddToWatchlistCommand(string Symbol);

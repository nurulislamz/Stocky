namespace stockyapi.Application.Portfolio.ZHelperTypes;

public record DeleteConfirmationDto(Guid Id, string Ticker, DateTimeOffset DeletedAt);
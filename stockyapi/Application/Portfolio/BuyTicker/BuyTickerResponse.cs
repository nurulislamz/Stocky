using stockyapi.Application.Portfolio;

namespace stockyapi.Responses;

public record BuyTickerResponse(TradeConfirmationDto BoughtTickers);
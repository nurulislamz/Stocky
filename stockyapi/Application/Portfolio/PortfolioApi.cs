using System.Collections.Immutable;
using stockyapi.Application.Commands.Portfolio;
using stockyapi.Application.Portfolio.BuyTicker;
using stockyapi.Application.Portfolio.DeleteHolding;
using stockyapi.Application.Portfolio.GetHoldings;
using stockyapi.Application.Portfolio.ListHoldings;
using stockyapi.Application.Portfolio.SellTicker;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.PortfolioRepository;
using stockyapi.Repository.PortfolioRepository.Types;
using stockymodels.models;

namespace stockyapi.Application.Portfolio;

public sealed class PortfolioApi : IPortfolioApi
{
    private readonly IUserContext _userContext;
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioApi(IUserContext userContext, IPortfolioRepository portfolioRepository)
    {
        _userContext = userContext;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<Result<ListHoldingsResponse>> ListHoldings(CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.ListAllHoldingsAsync(_userContext.UserId, cancellationToken);
        
        var allHoldings = portfolio.Holdings;
        var processedHoldings = allHoldings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new ListHoldingsResponse(portfolio.TotalValue, portfolio.CashBalance, portfolio.InvestedAmount, processedHoldings);
    }

    public async Task<Result<GetHoldingsResponse>> GetHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetHoldingsByIdAsync(_userContext.UserId, requestedHoldingIds, cancellationToken);
        if (portfolio.MissingIdsOrTickers.Any())
            return new ValidationFailure422($"These tickers were not found in your portfolio: {portfolio.MissingIdsOrTickers}");
        
        var processedHoldings = portfolio.Holdings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new GetHoldingsResponse(processedHoldings);
    }
    
    public async Task<Result<GetHoldingsResponse>> GetHoldingsByTicker(string[] requestedTickers, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetHoldingsByTickerAsync(_userContext.UserId, requestedTickers, cancellationToken);
        if (portfolio.MissingIdsOrTickers.Any())
            return new ValidationFailure422($"These tickers were not found in your portfolio: {portfolio.MissingIdsOrTickers}");
        
        var processedHoldings = portfolio.Holdings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new GetHoldingsResponse(processedHoldings);
    }

    public async Task<Result<BuyTickerResponse>> BuyTicker(BuyTickerRequest request, CancellationToken cancellationToken)
    {
        var portfolio = await _portfolioRepository.GetPortfolioFromUserIdAsync(_userContext.UserId, cancellationToken);
        
        var totalCost = request.Price * request.Quantity;
        if (totalCost > portfolio.CashBalance)
            return new ConflictFailure409(
                $"Insufficient Funds. Required balance (${totalCost}) exceeds available funds {portfolio.CashBalance}");
        
        var command = new StockBoughtCommand(request.Symbol, request.Quantity, request.Price);
        var result = await _portfolioRepository.BuyHoldingAsync(_userContext.UserId, command, cancellationToken);
        
        return new BuyTickerResponse(TradeResultToDto(result));
    }
    
    public async Task<Result<SellTickerResponse>> SellTicker(SellTickerRequest request, CancellationToken cancellationToken)
    {
        var holdingResponse = await _portfolioRepository.GetHoldingsByTickerAsync(_userContext.UserId, [request.Symbol], cancellationToken);
        if (holdingResponse.MissingIdsOrTickers.Any())
            return new ValidationFailure422($"These tickers were not found in your portfolio: {holdingResponse.MissingIdsOrTickers}");

        var holding = holdingResponse.Holdings.Single();
        if (holding.Shares < request.Quantity)
            return new ConflictFailure409(
                $"Insufficient Position: You are attempting to sell {request.Quantity} shares of {request.Symbol}, " +
                $"but your current holding is only {holding.Shares} shares.");
        
        var command = new StockSoldCommand(request.Symbol, request.Quantity, request.Price);
        var result = await _portfolioRepository.SellHoldingAsync(_userContext.UserId, command, cancellationToken);
        
        return new SellTickerResponse(TradeResultToDto(result));
    }

    public async Task<Result<DeleteHoldingsResponse>> DeleteHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken)
    {
        var validateHoldings = await _portfolioRepository.GetHoldingsByIdAsync(_userContext.UserId, requestedHoldingIds, cancellationToken);
        if (validateHoldings.MissingIdsOrTickers.Any())
            return new ValidationFailure422($"These tickers were not found in your portfolio: {validateHoldings.MissingIdsOrTickers}");

        var holdings = validateHoldings.Holdings;
        var deletedIds = await _portfolioRepository.DeleteHoldingsAsync(_userContext.UserId, holdings, cancellationToken);

        var confirmations = holdings.Select(h => new DeleteConfirmationDto(h.Id, h.Ticker, DateTimeOffset.UtcNow));
        return new DeleteHoldingsResponse(confirmations);
    }
    
    public async Task<Result<DeleteHoldingsResponse>> DeleteHoldingsByTicker(string[] requestedTickers, CancellationToken cancellationToken)
    {
        var holdingsResult = await _portfolioRepository.GetHoldingsByTickerAsync(_userContext.UserId, requestedTickers, cancellationToken);
        if (holdingsResult.MissingIdsOrTickers.Any())
            return new ValidationFailure422($"These tickers were not found in your portfolio: {holdingsResult.MissingIdsOrTickers}");
        
        var holdings = holdingsResult.Holdings;
        var deletedIds = await _portfolioRepository.DeleteHoldingsAsync(_userContext.UserId, holdings, cancellationToken);

        var confirmations = holdings.Select(h => new DeleteConfirmationDto(h.Id, h.Ticker, DateTimeOffset.UtcNow));
        return new DeleteHoldingsResponse(confirmations);
    }

    Task<Result<GetHoldingsResponse>> IPortfolioApi.UpdateHoldings(string request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private HoldingDto ProcessHoldingModelsToHoldings(StockHoldingModel holding)
        => new HoldingDto
        {
            Ticker = holding.Ticker,
            Quantity = holding.Shares,
            AverageBuyPrice = holding.AverageCost,
            TotalCost = holding.Shares * holding.AverageCost,
            LastUpdatedTime = holding.UpdatedAt
        };
    
    private TradeConfirmationDto TradeResultToDto(TradeResult result)
        => new TradeConfirmationDto
        {
            Ticker = result.Ticker,
            QuantityBought = result.Quantity,
            ExecutionPrice = result.Price,
            NewAveragePrice = result.NewAverageCost,
            TotalCost = result.Quantity * result.Price,
            NewCashBalance = result.UpdatedPortfolio.CashBalance,
            NewInvestedAmount = result.UpdatedPortfolio.InvestedAmount,
            NewTotalValue = result.UpdatedPortfolio.TotalValue,
            TransactionId = result.EventId,
            Timestamp = DateTime.UtcNow
        };
}

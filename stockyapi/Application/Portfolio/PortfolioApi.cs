using System.Collections.Immutable;
using stockyapi.Application.Portfolio.BuyTicker;
using stockyapi.Application.Portfolio.DeleteHolding;
using stockyapi.Application.Portfolio.GetHoldings;
using stockyapi.Application.Portfolio.ListHoldings;
using stockyapi.Application.Portfolio.SellTicker;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Middleware;
using stockyapi.Repository.PortfolioRepository;
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
        var portfolioResult = await _portfolioRepository.ListAllHoldingsAsync(_userContext.UserId, cancellationToken);
        if (portfolioResult.IsFailure)
            return Result<ListHoldingsResponse>.Fail(portfolioResult.Failure);
        var portfolio = portfolioResult.Value;
        var allHoldings = portfolioResult.Value.Holdings;

        var processedHoldings = allHoldings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new ListHoldingsResponse(portfolio.TotalValue, portfolio.CashBalance, portfolio.InvestedAmount, processedHoldings);
    }

    public async Task<Result<GetHoldingsResponse>> GetHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken)
    {
        var portfolioResult = await _portfolioRepository.GetHoldingsByIdAsync(_userContext.UserId, requestedHoldingIds, cancellationToken);
        if (portfolioResult.IsFailure)
            return portfolioResult.Failure;
        
        var allHoldings = portfolioResult.Value;
        var processedHoldings = allHoldings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new GetHoldingsResponse(processedHoldings);
    }
    
    public async Task<Result<GetHoldingsResponse>> GetHoldingsByTicker(string[] requestedTickers, CancellationToken cancellationToken)
    {
        var portfolioResult = await _portfolioRepository.GetHoldingsByTickerAsync(_userContext.UserId, requestedTickers, cancellationToken);
        if (portfolioResult.IsFailure)
            return portfolioResult.Failure;
        
        var allHoldings = portfolioResult.Value;
        var processedHoldings = allHoldings.Select(ProcessHoldingModelsToHoldings).ToImmutableArray();
        return new GetHoldingsResponse(processedHoldings);
    }

    public async Task<Result<BuyTickerResponse>> BuyTicker(BuyTickerRequest request, CancellationToken cancellationToken)
    {
        var portfolioResult = await _portfolioRepository.GetPortfolioModelFromUserIdAsync(_userContext.UserId, cancellationToken);
        if (portfolioResult.IsFailure)
            return portfolioResult.Failure;
        var portfolio = portfolioResult.Value;
        
        // Validate purchase
        var totalCost = request.Price * request.Quantity;
        if (totalCost > portfolio.CashBalance)
            return new ConflictFailure409(
                $"Insufficient Funds. Required balance (${totalCost}) exceeds available funds {portfolio.CashBalance}");
        
        // Create transaction
        var command = new BuyOrderCommand(portfolio.Id, request.Symbol, request.Quantity, request.Price);
        var buyResult = await _portfolioRepository.BuyHoldingAsync(_userContext.UserId, command, cancellationToken);
        
        if (buyResult.IsFailure)
            return buyResult.Failure;
        var transaction = buyResult.Value.Item1;
        var updatedPortfolio = buyResult.Value.Item2;
        
        return new BuyTickerResponse(ProcessAssetTransactionModelsToHoldings(transaction, updatedPortfolio));
    }
    
    public async Task<Result<SellTickerResponse>> SellTicker(SellTickerRequest request, CancellationToken cancellationToken)
    {
        var portfolioResult = await _portfolioRepository.GetPortfolioModelFromUserIdAsync(_userContext.UserId, cancellationToken);
        if (portfolioResult.IsFailure)
            return portfolioResult.Failure;
        
        var portfolio = portfolioResult.Value;
        // Validate User has a holding in that Ticker
        var holdingExist = await _portfolioRepository.GetHoldingsByTickerAsync(_userContext.UserId, [request.Symbol], cancellationToken);
        if (holdingExist.IsFailure)
            return holdingExist.Failure;

        var holding = holdingExist.Value.First();
        if (holding.Shares < request.Quantity)
            return new ConflictFailure409(
                $"Insufficient Position: You are attempting to sell {request.Quantity} shares of {request.Symbol}, " +
                $"but your current holding is only {holding.Shares} shares.");
        
        // Create sellOrderCommand
        var command = new SellOrderCommand(portfolio.Id, request.Symbol, request.Quantity, request.Price);
        
        var sellResult = await _portfolioRepository.SellHoldingAsync(_userContext.UserId, command, cancellationToken);
        if (sellResult.IsFailure)
            return sellResult.Failure;
        
        var transaction = sellResult.Value.Item1;
        var updatedPortfolio = sellResult.Value.Item2;
        
        return new SellTickerResponse(ProcessAssetTransactionModelsToHoldings(transaction, updatedPortfolio));
    }

    public async Task<Result<DeleteHoldingsResponse>> DeleteHoldingsById(Guid[] requestedHoldingIds, CancellationToken cancellationToken)
    {
        var deletedHoldingsResponse = await _portfolioRepository.DeleteHoldingsByIdAsync(_userContext.UserId, requestedHoldingIds, cancellationToken);
        if (deletedHoldingsResponse.IsFailure)
            return deletedHoldingsResponse.Failure;

        var deletedHoldings = deletedHoldingsResponse.Value;
        return new DeleteHoldingsResponse(ProcessHoldingsModelsToDeleteConfirmationDto(deletedHoldings));
    }
    
    public async Task<Result<DeleteHoldingsResponse>> DeleteHoldingsByTicker(string[] requestedTickerIds, CancellationToken cancellationToken)
    {
        var deletedHoldingsResponse = await _portfolioRepository.DeleteHoldingsByTickerAsync(_userContext.UserId, requestedTickerIds, cancellationToken);
        if (deletedHoldingsResponse.IsFailure)
            return deletedHoldingsResponse.Failure;

        var deletedHoldings = deletedHoldingsResponse.Value;
        return new DeleteHoldingsResponse(ProcessHoldingsModelsToDeleteConfirmationDto(deletedHoldings));
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
    
    private TradeConfirmationDto ProcessAssetTransactionModelsToHoldings(AssetTransactionModel transactionModel, PortfolioModel updatedPortfolio)
        => new TradeConfirmationDto
        {
            Ticker = transactionModel.Ticker,
            QuantityBought = transactionModel.Quantity,
            ExecutionPrice = transactionModel.Price,
            NewAveragePrice = transactionModel.NewAverageCost,
            TotalCost = transactionModel.Quantity * transactionModel.Price,
            NewCashBalance = updatedPortfolio.CashBalance,
            NewInvestedAmount = updatedPortfolio.InvestedAmount,
            NewTotalValue = updatedPortfolio.TotalValue,
            TransactionId = transactionModel.Id,
            Timestamp = transactionModel.UpdatedAt
        };

    private IEnumerable<DeleteConfirmationDto> ProcessHoldingsModelsToDeleteConfirmationDto(IEnumerable<StockHoldingModel> holdings)
    {
        return holdings.Select(h => new DeleteConfirmationDto(h.Id, h.Ticker, DateTimeOffset.UtcNow));
    }
}
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using stockyapi.Requests;
using stockyapi.Responses;
using stockymodels.Data;
using stockymodels.models;

namespace stockyapi.Services;

public class PortfolioService : IPortfolioService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _context;

    public PortfolioService(IHttpContextAccessor httpContextAccessor, ITokenService tokenService, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<PortfolioModel?> FetchUserPortfolio()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return null;

        return await _context.Portfolios
            .Where(x => x.UserId.ToString() == userId)
            .Include(portfolioModel => portfolioModel.StockHoldings)
            .FirstOrDefaultAsync();
    }

    public bool FetchCheckYouHaveEnoughMoney(decimal price, decimal quantity, decimal currentCash)
    {
        decimal totalCost = price * quantity;
        return totalCost > currentCash ;
    }

    public GetPortfolioResponse CreatePortfolioResponse(PortfolioModel? portfolio)
    {
        if (portfolio == null)
        {
            return new GetPortfolioResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "User portfolio not found"
            };
        }

        var portfolioItems = portfolio.StockHoldings.Select(holding => new PortfolioItem
        {
            Symbol = holding.Symbol,
            Quantity = holding.Shares,
            AverageBuyPrice = holding.AverageCost,
            CurrentPrice = holding.CurrentPrice,
            TotalValue = holding.Shares * holding.CurrentPrice,
            ProfitLoss = (holding.CurrentPrice - holding.AverageCost) * holding.Shares,
            ProfitLossPercentage = holding.AverageCost != 0
                ? ((holding.CurrentPrice - holding.AverageCost) / holding.AverageCost) * 100
                : 0,
            LastUpdatedTime = holding.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? holding.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return new GetPortfolioResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new PortfolioData
            {
                TotalValue = portfolio.TotalValue,
                CashBalance = portfolio.CashBalance,
                InvestedAmount = portfolio.InvestedAmount,
                Items = portfolioItems,
            }
        };
    }

    public async Task<BuyTickerResponse> BuyTickerPortfolio(BuyTickerRequest request)
    {
        var portfolioData = await FetchUserPortfolio();
        if (portfolioData == null)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }
        
        // 2. Validate purchase
        var totalCost = request.Price * request.Quantity;
        if (totalCost > portfolioData.CashBalance)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 400,
                Message = "Insufficient funds"
            };
        }

        // 3. Create transaction
        var transaction = new TransactionModel
        {
            PortfolioId = portfolioData.Id,
            Symbol = request.Symbol,
            Type = TransactionType.Buy,
            Shares = (int)request.Quantity,
            Price = request.Price,
            TotalAmount = totalCost,
            Status = TransactionStatus.Completed,
            OrderType = OrderType.Market
        };

        // 4. Update portfolio
        portfolioData.CashBalance -= totalCost;
        portfolioData.InvestedAmount += totalCost;

        // 5. Update or create stock holding
        var existingHolding = portfolioData.StockHoldings
            .FirstOrDefault(h => h.Symbol == request.Symbol);

        if (existingHolding != null)
        {
            // Update existing holding
            var newTotalShares = existingHolding.Shares + (int)request.Quantity;
            var newTotalCost = (existingHolding.AverageCost * existingHolding.Shares) + totalCost;
            existingHolding.Shares = newTotalShares;
            existingHolding.AverageCost = newTotalCost / newTotalShares;
            existingHolding.CurrentPrice = request.Price;
        }
        else
        {
            // Create new holding
            var newHolding = new StockHoldingModel
            {
                PortfolioId = portfolioData.Id,
                Symbol = request.Symbol,
                Shares = request.Quantity,
                AverageCost = request.Price,
                CurrentPrice = request.Price
            };
            portfolioData.StockHoldings.Add(newHolding);
        }

        // 6. Save changes
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // 7. Return response
        return new BuyTickerResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new BuyTickerData
            {
                Symbol = request.Symbol,
                Quantity = request.Quantity,
                Price = request.Price,
                TotalCost = totalCost,
                RemainingCashBalance = portfolioData.CashBalance,
                TransactionTime = DateTime.UtcNow,
                TransactionId = transaction.Id.ToString(),
                Status = transaction.Status
            }
        };
    }

    public Task<SellTickerResponse> SellTickerPortfolio(SellTickerRequest request)
    {
        throw new NotImplementedException();
    }

    public TransactionModel CreateBuyTransaction(PortfolioModel portfolioData, BuyTickerRequest request)
    {
        return new TransactionModel
        {
            PortfolioId = portfolioData.Id,
            Symbol = request.Symbol,
            Type = TransactionType.Buy,
            Shares = request.Quantity,
            Price = request.Price,
            TotalAmount = request.Quantity*request.Price,
            Status = TransactionStatus.Completed,
            OrderType = OrderType.Market
        };
    }
    
    public TransactionModel CreateSellTransaction(PortfolioModel portfolioData, SellTickerRequest request)
    {
        return new TransactionModel
        {
            PortfolioId = portfolioData.Id,
            Symbol = request.Symbol,
            Type = TransactionType.Sell,
            Shares = request.Quantity,
            Price = request.Price,
            TotalAmount = request.Quantity*request.Price,
            Status = TransactionStatus.Completed,
            OrderType = OrderType.Market
        };
    }
}
using System.Security.Claims;
using stockyapi.Repository.Portfolio;
using stockyapi.Requests;
using stockyapi.Responses;
using stockymodels.models;

namespace stockyapi.Services;

public interface IPortfolioService
{
    public Task<UserPortfolioResponse> GetUserPortfolio();
    public Task<BuyTickerResponse> BuyTickerInPortfolio(BuyTickerRequest request);
    public Task<SellTickerResponse> SellTickerInPortfolio(SellTickerRequest request);
    public Task<AddFundsResponse> AddFunds(AddFundsRequest request);
    public Task<SetFundsResponse> SetFunds(SetFundsRequest request);
    public Task<SubtractFundsResponse> SubtractFunds(SubtractFundsRequest request);
}

public class PortfolioService : IPortfolioService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioService(
        IHttpContextAccessor httpContextAccessor,
        IPortfolioRepository portfolioRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _portfolioRepository = portfolioRepository;
    }

    public async Task<UserPortfolioResponse> GetUserPortfolio()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new UserPortfolioResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new UserPortfolioResponse
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
            LastUpdatedTime = holding.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") ?? holding.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return new UserPortfolioResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new PortfolioData
            {
                TotalValue = portfolio.TotalValue,
                CashBalance = portfolio.CashBalance,
                InvestedAmount = portfolio.InvestedAmount,
                Items = portfolioItems
            }
        };
    }

    public async Task<BuyTickerResponse> BuyTickerInPortfolio(BuyTickerRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }

        if (request.Quantity <= 0)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Quantity must be greater than 0"
            };
        }

        if (request.Price <= 0)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Price must be greater than 0"
            };
        }

        // Validate purchase
        var totalCost = request.Price * request.Quantity;
        if (totalCost > portfolio.CashBalance)
        {
            return new BuyTickerResponse
            {
                Success = false,
                StatusCode = 400,
                Message = "Insufficient funds"
            };
        }

        // Create transaction
        var transaction = new TransactionModel
        {
            PortfolioId = portfolio.Id,
            Symbol = request.Symbol,
            Type = TransactionType.Buy,
            Shares = request.Quantity,
            Price = request.Price,
            TotalAmount = totalCost,
            Status = TransactionStatus.Completed,
            OrderType = OrderType.Market,
            LastPriceUpdate = DateTime.UtcNow
        };

        // Update portfolio balance
        portfolio.CashBalance -= totalCost;
        portfolio.InvestedAmount += totalCost;

        // Update or create stock holding
        var existingHolding = await _portfolioRepository.GetHoldingAsync(portfolio.Id, request.Symbol);
        if (existingHolding != null)
        {
            // Update existing holding
            var newTotalShares = existingHolding.Shares + request.Quantity;
            var newTotalCost = (existingHolding.AverageCost * existingHolding.Shares) + totalCost;
            existingHolding.Shares = newTotalShares;
            existingHolding.AverageCost = newTotalCost / newTotalShares;
            existingHolding.CurrentPrice = request.Price;
            await _portfolioRepository.UpdateHoldingAsync(existingHolding);
        }
        else
        {
            // Create new holding
            var newHolding = new StockHoldingModel
            {
                PortfolioId = portfolio.Id,
                Symbol = request.Symbol,
                Shares = request.Quantity,
                AverageCost = request.Price,
                CurrentPrice = request.Price
            };
            await _portfolioRepository.AddHoldingAsync(newHolding);
        }

        // Save transaction and update portfolio
        await _portfolioRepository.AddTransactionAsync(transaction);
        await _portfolioRepository.UpdateAsync(portfolio);

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
                RemainingCashBalance = portfolio.CashBalance,
                TransactionTime = DateTime.UtcNow,
                TransactionId = transaction.Id.ToString(),
                Status = transaction.Status
            }
        };
    }

    public async Task<SellTickerResponse> SellTickerInPortfolio(SellTickerRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new SellTickerResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new SellTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }

        var total = request.Price * request.Quantity;

        // Create transaction
        var transaction = new TransactionModel
        {
            PortfolioId = portfolio.Id,
            Symbol = request.Symbol,
            Type = TransactionType.Sell,
            Shares = request.Quantity,
            Price = request.Price,
            TotalAmount = total,
            Status = TransactionStatus.Completed,
            OrderType = OrderType.Market,
            LastPriceUpdate = DateTime.UtcNow
        };

        // Update portfolio
        portfolio.CashBalance += total;
        portfolio.InvestedAmount -= total;

        // Update stock holding
        var existingHolding = await _portfolioRepository.GetHoldingAsync(portfolio.Id, request.Symbol);
        if (existingHolding == null)
        {
            return new SellTickerResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Stock is not owned"
            };
        }

        if (existingHolding.Shares < request.Quantity)
        {
            return new SellTickerResponse
            {
                Success = false,
                StatusCode = 400,
                Message = "Insufficient shares to sell"
            };
        }

        // Update holding
        var newTotalShares = existingHolding.Shares - request.Quantity;
        var newTotalCost = (existingHolding.AverageCost * existingHolding.Shares) - total;
        existingHolding.Shares = newTotalShares;
        existingHolding.AverageCost = newTotalShares == 0 ? 0 : newTotalCost / newTotalShares;
        existingHolding.CurrentPrice = request.Price;

        // Save changes
        await _portfolioRepository.AddTransactionAsync(transaction);
        await _portfolioRepository.UpdateHoldingAsync(existingHolding);
        await _portfolioRepository.UpdateAsync(portfolio);

        return new SellTickerResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new SellTickerData
            {
                Symbol = request.Symbol,
                Quantity = request.Quantity,
                Price = request.Price,
                TotalCost = total,
                RemainingCashBalance = portfolio.CashBalance,
                TransactionTime = DateTime.UtcNow,
                TransactionId = transaction.Id.ToString(),
                Status = transaction.Status
            }
        };
    }

    public async Task<AddFundsResponse> AddFunds(AddFundsRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new AddFundsResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new AddFundsResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }

        var newBalance = await _portfolioRepository.AddCashAsync(portfolio.Id, request.Amount);
        return new AddFundsResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new AddFundsData
            {
                NewBalance = newBalance
            }
        };
    }

    public async Task<SubtractFundsResponse> SubtractFunds(SubtractFundsRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new SubtractFundsResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new SubtractFundsResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }

        var newBalance = await _portfolioRepository.SubtractCashAsync(portfolio.Id, request.Amount);
        return new SubtractFundsResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new SubtractFundsData
            {
                NewBalance = newBalance
            }
        };
    }

    public async Task<SetFundsResponse> SetFunds(SetFundsRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return new SetFundsResponse
            {
                Success = false,
                StatusCode = 401,
                Message = "User not authenticated"
            };
        }

        var portfolio = await _portfolioRepository.GetByUserIdAsync(Guid.Parse(userId));
        if (portfolio == null)
        {
            return new SetFundsResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "Portfolio not found"
            };
        }

        var newBalance = await _portfolioRepository.SetCashAsync(portfolio.Id, request.Amount);
        return new SetFundsResponse
        {
            Success = true,
            StatusCode = 200,
            Data = new SetFundsData
            {
                NewBalance = newBalance
            }
        };
    }
}
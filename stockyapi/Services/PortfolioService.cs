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

    public async Task<GetPortfolioResponse> GetUserPortfolio()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return new GetPortfolioResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "User not found"
            };
        }
        var portfolio = _context.Portfolios
            .Where(x => x.UserId == userId)
            .Include(portfolioModel => portfolioModel.StockHoldings)
            .FirstOrDefaultAsync().Result;

        if (portfolio == null)
        {
            return new GetPortfolioResponse
            {
                Success = false,
                StatusCode = 404,
                Message = "User porfolio not found"
            };
        }

        decimal totalValue = portfolio.TotalValue;
        decimal cashBalance = portfolio.CashBalance;
        decimal investedAmount = portfolio.InvestedAmount;
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
                TotalValue = totalValue,
                CashBalance = cashBalance,
                InvestedAmount = investedAmount,
                Items = portfolioItems,
            }
        };
    }

    public async Task<(bool success, string? token, string? error)> RegisterUser(string firstName, string surname, string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return (false, null, "Email already registered");

        var user = new UserModel
        {
            FirstName = firstName,
            Surname = surname,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.User, // Default role
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenService.CreateToken(user);
        return (true, token, null);
    }
}
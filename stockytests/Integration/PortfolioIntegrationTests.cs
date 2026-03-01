using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Application.Portfolio;
using stockyapi.Application.Portfolio.BuyTicker;
using stockyapi.Application.Portfolio.SellTicker;
using stockyapi.Repository.PortfolioRepository;
using stockymodels.Data;
using stockymodels.models;
using stockymodels.Models.Enums;
using stockytests.Helpers;

namespace stockytests.Integration;

[TestFixture]
[Category("Integration")]
public class PortfolioIntegrationTests
{
    private SqliteTestSession _session = null!;
    private PortfolioApi _portfolioApi = null!;
    private TestUserContext _userContext = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        // Infrastructure is created once
        _session = await SqliteTestSession.CreateAsync();
    }
    
    private async Task InitialiseTestAsync(decimal cash = 0m, decimal invested = 0m)
    {
        await _session.SetupUser(cash, invested);
        _userContext = new TestUserContext(true, _session.UserId, _session.UserEmail, "Integration", "Tester", "User");

        var portfolioRepo = new PortfolioRepository(_session.Context, NullLogger<PortfolioRepository>.Instance);
        _portfolioApi = new PortfolioApi(_userContext, portfolioRepo);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _session.DisposeAsync();
    }

    [Test]
    public async Task ListHoldings_NoHoldings_ReturnsEmpty()
    {
        // Arrange
        await InitialiseTestAsync();

        // Act
        var result = await _portfolioApi.ListHoldings(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Items, Is.Empty);
    }
    
    [Test]
    public async Task BuyTicker_ValidPurchase_CreatesHoldingAndTransaction()
    {
        // Arrange
        await InitialiseTestAsync(cash: 1000m);
        var request = new BuyTickerRequest
        {
            Symbol = "AAPL",
            Quantity = 5,
            Price = 100,
        };

        // Act
        var result = await _portfolioApi.BuyTicker(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        var holding = _session.Context.StockHoldings.FirstOrDefault(h => h.Ticker == "AAPL");
        Assert.That(holding, Is.Not.Null);
        Assert.That(holding.Shares, Is.EqualTo(5));

        var transaction = _session.Context.AssetTransactions.FirstOrDefault(t => t.Ticker == "AAPL");
        Assert.That(transaction, Is.Not.Null);
        Assert.That(transaction.Type, Is.EqualTo(TransactionType.Buy));
        Assert.That(transaction.Quantity, Is.EqualTo(5));

        var portfolio = await _session.Context.Portfolios.FindAsync(_session.PortfolioId);
        Assert.That(portfolio!.CashBalance, Is.EqualTo(1000m - 5 * 100));
    }

    [Test]
    public async Task SellTicker_ValidSale_UpdatesHoldingAndCreatesTransaction()
    {
        // Arrange
        await InitialiseTestAsync(cash: 500m);
        
        var initialHolding = new StockHoldingModel
        {
            PortfolioId = _session.PortfolioId,
            Ticker = "MSFT",
            Shares = 10,
            AverageCost = 90
        };
        _session.Context.StockHoldings.Add(initialHolding);
        await _session.Context.SaveChangesAsync();

        var request = new SellTickerRequest
        {
            Symbol = "MSFT",
            Quantity = 3,
            Price = 110,
        };

        // Act
        var result = await _portfolioApi.SellTicker(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        var holding = _session.Context.StockHoldings.FirstOrDefault(h => h.Ticker == "MSFT");
        Assert.That(holding, Is.Not.Null);
        Assert.That(holding.Shares, Is.EqualTo(7));

        var transaction = _session.Context.AssetTransactions.FirstOrDefault(t => t.Ticker == "MSFT");
        Assert.That(transaction, Is.Not.Null);
        Assert.That(transaction.Type, Is.EqualTo(TransactionType.Sell));
        Assert.That(transaction.Quantity, Is.EqualTo(3));
        
        var portfolio = await _session.Context.Portfolios.FindAsync(_session.PortfolioId);
        Assert.That(portfolio!.CashBalance, Is.EqualTo(500m + 3 * 110));
    }
    
    [Test]
    public async Task GetHoldingsById_ExistingHolding_ReturnsHolding()
    {
        // Arrange
        await InitialiseTestAsync();
        var holdingId = Guid.NewGuid();
        var initialHolding = new StockHoldingModel
        {
            Id = holdingId,
            PortfolioId = _session.PortfolioId,
            Ticker = "GOOG",
            Shares = 10,
            AverageCost = 2000
        };
        _session.Context.StockHoldings.Add(initialHolding);
        await _session.Context.SaveChangesAsync();

        // Act
        var result = await _portfolioApi.GetHoldingsById(new[] { holdingId }, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var holding = result.Value.Items.FirstOrDefault();
        Assert.That(holding, Is.Not.Null);
        Assert.That(holding.Ticker, Is.EqualTo("GOOG"));
    }

    [Test]
    public async Task GetHoldingsByTicker_ExistingHolding_ReturnsHolding()
    {
        // Arrange
        await InitialiseTestAsync();
        var initialHolding = new StockHoldingModel
        {
            PortfolioId = _session.PortfolioId,
            Ticker = "AMD",
            Shares = 15,
            AverageCost = 150
        };
        _session.Context.StockHoldings.Add(initialHolding);
        await _session.Context.SaveChangesAsync();

        // Act
        var result = await _portfolioApi.GetHoldingsByTicker(new[] { "AMD" }, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var holding = result.Value.Items.FirstOrDefault();
        Assert.That(holding, Is.Not.Null);
        Assert.That(holding.Ticker, Is.EqualTo("AMD"));
    }

    [Test]
    public async Task DeleteHoldingById_ExistingHolding_DeletesSuccessfully()
    {
        // Arrange
        await InitialiseTestAsync();
        var holdingId = Guid.NewGuid();
        var initialHolding = new StockHoldingModel
        {
            Id = holdingId,
            PortfolioId = _session.PortfolioId,
            Ticker = "NVDA",
            Shares = 5,
            AverageCost = 800
        };
        _session.Context.StockHoldings.Add(initialHolding);
        await _session.Context.SaveChangesAsync();

        // Act
        var result = await _portfolioApi.DeleteHoldingsById(new[] { holdingId }, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var holding = await _session.Context.StockHoldings.FindAsync(holdingId);
        Assert.That(holding, Is.Null);
    }
    
    [Test]
    public async Task DeleteHoldingByTicker_ExistingHolding_DeletesSuccessfully()
    {
        // Arrange
        await InitialiseTestAsync();
        var initialHolding = new StockHoldingModel
        {
            PortfolioId = _session.PortfolioId,
            Ticker = "TSLA",
            Shares = 2,
            AverageCost = 250
        };
        _session.Context.StockHoldings.Add(initialHolding);
        await _session.Context.SaveChangesAsync();

        // Act
        var result = await _portfolioApi.DeleteHoldingsByTicker(new[] { "TSLA" }, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var holding = _session.Context.StockHoldings.FirstOrDefault(h => h.Ticker == "TSLA");
        Assert.That(holding, Is.Null);
    }
}

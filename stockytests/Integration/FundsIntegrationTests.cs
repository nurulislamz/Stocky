using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Application.Funds;
using stockyapi.Application.Funds.AddFunds;
using stockyapi.Application.Funds.SubtractFunds;
using stockyapi.Repository.Funds;
using stockyapi.Repository.PortfolioRepository;
using stockyapi.Middleware;
using stockytests.Helpers;

namespace stockytests.Integration;

[TestFixture]
[Category("Integration")]
public class FundsIntegrationTests
{
    private SqliteTestSession _session;
    private FundsApi _fundsApi = null!;
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

        var fundsRepo = new FundsRepository(_session.Context, NullLogger<FundsRepository>.Instance);
        _fundsApi = new FundsApi(_userContext, fundsRepo);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _session.DisposeAsync();
    }

    [Test]
    public async Task GetFunds_UserHasFunds_ReturnsCorrectBalances()
    {
        // Arrange
        await InitialiseTestAsync(100, 50);
        
        // Act
        var result = await _fundsApi.GetFunds(CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.CashBalance, Is.EqualTo(100m));
        Assert.That(result.Value.InvestedAmount, Is.EqualTo(50m));
        Assert.That(result.Value.TotalValue, Is.EqualTo(150m));
    }

    [Test]
    public async Task DepositFunds_ValidAmount_IncreasesCashBalance()
    {
        // Arrange
        await InitialiseTestAsync(150, 50);
        var request = new DepositFundsRequest { Amount = 50m };

        // Act
        var result = await _fundsApi.DepositFunds(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.CashBalance, Is.EqualTo(200m));
        Assert.That(result.Value.InvestedAmount, Is.EqualTo(50m));
        Assert.That(result.Value.TotalValue, Is.EqualTo(250m));

        // Verify persistence
        var portfolio = await _session.Context.Portfolios.FirstOrDefaultAsync(p => p.UserId == _userContext.UserId);
        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.CashBalance, Is.EqualTo(200m));
    }

    [Test]
    public async Task WithdrawFunds_ValidAmount_DecreasesCashBalance()
    {
        // Arrange
        await InitialiseTestAsync(100, 50);
        var request = new WithdrawFundsRequest { Amount = 40m };

        // Act
        var result = await _fundsApi.WithdrawFunds(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.CashBalance, Is.EqualTo(60m));
        Assert.That(result.Value.InvestedAmount, Is.EqualTo(50m));
        Assert.That(result.Value.TotalValue, Is.EqualTo(110m));

        // Verify persistence
        var portfolio = await _session.Context.Portfolios.FirstOrDefaultAsync(p => p.UserId == _userContext.UserId);
        Assert.That(portfolio, Is.Not.Null);
        Assert.That(portfolio.CashBalance, Is.EqualTo(60m));
    }

    [Test]
    public async Task WithdrawFunds_InsufficientFunds_ReturnsFailure()
    {
        // Arrange
        // Re-setup user with low balance for this specific test if needed,
        // or just use a large withdrawal amount.
        // Current balance is 100m.
        await InitialiseTestAsync(100, 50);
        var request = new WithdrawFundsRequest { Amount = 200m };

        // Act
        var result = await _fundsApi.WithdrawFunds(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.TypeOf<BadRequestFailure400>());
        Assert.That(result.Failure.Detail, Does.StartWith("Insufficient funds."));
    }
}

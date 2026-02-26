using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using stockyapi.Application.Portfolio.ZHelperTypes;
using stockyapi.Repository.PortfolioRepository;
using stockytests.Helpers;

namespace stockytests.Integration.Repositories;

[TestFixture]
[Category("Integration")]
public class PortfolioRepositoryTests
{
    [Test]
    public async Task ListAllHoldingsAsync_WhenNoHoldings_ReturnsEmptyList()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(1000m, 0m);
        var repo = new PortfolioRepository(session.Context, NullLogger<PortfolioRepository>.Instance);

        var result = await repo.ListAllHoldingsAsync(session.UserId, CancellationToken.None);

        Assert.That(result.Holdings, Is.Empty);
        Assert.That(result.CashBalance, Is.EqualTo(1000m));
    }

    [Test]
    public async Task ListAllHoldingsAsync_AfterBuy_ReturnsHolding()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(1000m, 0m);
        var repo = new PortfolioRepository(session.Context, NullLogger<PortfolioRepository>.Instance);
        var command = new BuyOrderCommand(session.PortfolioId, "AAPL", 5, 100m);
        await repo.BuyHoldingAsync(session.UserId, command, CancellationToken.None);

        var result = await repo.ListAllHoldingsAsync(session.UserId, CancellationToken.None);
        var holdings = result.Holdings.ToList();

        Assert.That(holdings, Has.Count.EqualTo(1));
        Assert.That(holdings[0].Ticker, Is.EqualTo("AAPL"));
        Assert.That(holdings[0].Shares, Is.EqualTo(5));
    }

    [Test]
    public async Task GetHoldingsByIdAsync_WhenIdDoesNotExist_ReturnsMissingIds()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(1000m, 0m);
        var repo = new PortfolioRepository(session.Context, NullLogger<PortfolioRepository>.Instance);
        var nonExistentId = Guid.NewGuid();

        var result = await repo.GetHoldingsByIdAsync(session.UserId, new[] { nonExistentId }, CancellationToken.None);

        Assert.That(result.Holdings, Is.Empty);
        Assert.That(result.MissingIdsOrTickers, Has.Count.EqualTo(1));
        Assert.That(result.MissingIdsOrTickers[0], Is.EqualTo(nonExistentId));
    }

    [Test]
    public async Task GetHoldingsByTickerAsync_WhenTickerDoesNotExist_ReturnsMissingTickers()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(1000m, 0m);
        var repo = new PortfolioRepository(session.Context, NullLogger<PortfolioRepository>.Instance);

        var result = await repo.GetHoldingsByTickerAsync(session.UserId, new[] { "NOSUCH" }, CancellationToken.None);

        Assert.That(result.Holdings, Is.Empty);
        Assert.That(result.MissingIdsOrTickers, Has.Count.EqualTo(1));
        Assert.That(result.MissingIdsOrTickers[0], Is.EqualTo("NOSUCH"));
    }

    [Test]
    public async Task GetHoldingsByTickerAsync_WhenTickerExists_ReturnsHolding()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(1000m, 0m);
        var repo = new PortfolioRepository(session.Context, NullLogger<PortfolioRepository>.Instance);
        await repo.BuyHoldingAsync(session.UserId, new BuyOrderCommand(session.PortfolioId, "MSFT", 3, 200m), CancellationToken.None);

        var result = await repo.GetHoldingsByTickerAsync(session.UserId, new[] { "MSFT" }, CancellationToken.None);
        var holdings = result.Holdings.ToList();

        Assert.That(holdings, Has.Count.EqualTo(1));
        Assert.That(holdings[0].Ticker, Is.EqualTo("MSFT"));
        Assert.That(result.MissingIdsOrTickers, Is.Empty);
    }
}

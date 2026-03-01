using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using stockyapi.Repository.Funds;
using stockytests.Helpers;

namespace stockytests.Integration.Repositories;

[TestFixture]
[Category("Integration")]
public class FundsRepositoryTests
{
    [Test]
    public async Task GetFundsAsync_WhenUserHasPortfolio_ReturnsBalances()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(100m, 50m);
        var repo = new FundsRepository(session.Context, NullLogger<FundsRepository>.Instance);

        var balances = await repo.GetFundsAsync(session.UserId, CancellationToken.None);

        Assert.That(balances.CashBalance, Is.EqualTo(100m));
        Assert.That(balances.InvestedAmount, Is.EqualTo(50m));
        Assert.That(balances.TotalValue, Is.EqualTo(150m));
    }

    [Test]
    public async Task DepositFundsAsync_IncreasesCashAndTotalValue()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(200m, 0m);
        var repo = new FundsRepository(session.Context, NullLogger<FundsRepository>.Instance);

        var balances = await repo.DepositFundsAsync(session.UserId, 50m, CancellationToken.None);

        Assert.That(balances.CashBalance, Is.EqualTo(250m));
        Assert.That(balances.TotalValue, Is.EqualTo(250m));
    }

    [Test]
    public async Task WithdrawFundsAsync_DecreasesCashAndTotalValue()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser(200m, 0m);
        var repo = new FundsRepository(session.Context, NullLogger<FundsRepository>.Instance);

        var balances = await repo.WithdrawFundsAsync(session.UserId, 50m, CancellationToken.None);

        Assert.That(balances.CashBalance, Is.EqualTo(150m));
        Assert.That(balances.TotalValue, Is.EqualTo(150m));
    }

    [Test]
    public void GetFundsAsync_WhenPortfolioNotFound_Throws()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await using var session = await SqliteTestSession.CreateAsync();
            await session.SetupUser();
            var repo = new FundsRepository(session.Context, NullLogger<FundsRepository>.Instance);
            await repo.GetFundsAsync(Guid.NewGuid(), CancellationToken.None);
        });
    }
}

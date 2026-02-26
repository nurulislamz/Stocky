using NUnit.Framework;
using stockyapi.Repository.User;
using stockytests.Helpers;

namespace stockytests.Integration.Repositories;

[TestFixture]
[Category("Integration")]
public class UserRepositoryTests
{
    [Test]
    public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUser()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var user = await repo.GetUserByEmailAsync(session.UserEmail);

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Email, Is.EqualTo(session.UserEmail));
        Assert.That(user.Id, Is.EqualTo(session.UserId));
    }

    [Test]
    public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var user = await repo.GetUserByEmailAsync("nonexistent@example.com");

        Assert.That(user, Is.Null);
    }

    [Test]
    public async Task UserExistsByEmailAsync_WhenUserExists_ReturnsTrue()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var exists = await repo.UserExistsByEmailAsync(session.UserEmail);

        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task UserExistsByEmailAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var exists = await repo.UserExistsByEmailAsync("other@example.com");

        Assert.That(exists, Is.False);
    }

    [Test]
    public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var user = await repo.GetUserByIdAsync(session.UserId);

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Id, Is.EqualTo(session.UserId));
    }

    [Test]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        await using var session = await SqliteTestSession.CreateAsync();
        await session.SetupUser();
        var repo = new UserRepository(session.Context);

        var user = await repo.GetUserByIdAsync(Guid.NewGuid());

        Assert.That(user, Is.Null);
    }
}

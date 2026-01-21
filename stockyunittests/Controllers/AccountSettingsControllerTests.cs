using Microsoft.Extensions.Logging;
using Moq;
using stockyapi.Controllers;
using stockyunittests.Helpers;

namespace stockyunittests.Controllers;

[TestFixture]
public class AccountSettingsControllerTests
{
    private AccountSettingsController _controller = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _controller = ControllerTestHelpers.SetupController(
            new AccountSettingsController(Mock.Of<ILogger<AccountSettingsController>>()));
    }

    [Test]
    public void ChangePassword_ThrowsNotImplemented()
    {
        Assert.ThrowsAsync<NotImplementedException>(() => _controller.ChangePassword("request", Token));
    }

    [Test]
    public void SetOpenAiApiKey_ThrowsNotImplemented()
    {
        Assert.ThrowsAsync<NotImplementedException>(() => _controller.SetOpenAiApiKey("request", Token));
    }
}

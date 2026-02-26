using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using stockyapi.Services.YahooFinance;
using stockyapi.Services.YahooFinance.Helper;
using stockytests.Mocks;

namespace stockytests.Integration.MockTests;

[TestFixture]
[Category("Integration")]
public class YahooOptionsMockTests
{
    private IMemoryCache _memoryCache = null!;

    [SetUp]
    public void SetUp()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }

    [Test]
    public async Task GetOptions_WithAAPLMock_ReturnsOptionsChain()
    {
        // Arrange
        var mockCall = MockCallLoader.LoadMockCall("options-AAPL.json");
        var (service, _) = CreateService(mockCall);

        // Act
        var result = await service.GetOptionsAsync("AAPL", CancellationToken.None);

        // Assert
        if (!result.IsSuccess)
            Assert.Fail($"{result.Failure?.Title}: {result.Failure?.Detail}");
        Assert.That(result.Value.OptionChain, Is.Not.Null);
        Assert.That(result.Value.OptionChain.Result, Is.Not.Empty);

        var optionsResult = result.Value.OptionChain.Result.First();
        Assert.That(optionsResult.UnderlyingSymbol, Is.EqualTo("AAPL"));
        Assert.That(optionsResult.ExpirationDates, Is.Not.Empty);
        Assert.That(optionsResult.Strikes, Is.Not.Empty);
    }

    private (IYahooFinanceService Service, HttpClient HttpClient) CreateService(MockCallFile mockCall)
    {
        var handler = new MockHttpMessageHandler(mockCall);
        var httpClient = new HttpClient(handler);
        var logger = NullLogger<YahooApiServiceClient>.Instance;
        var serviceClient = new YahooApiServiceClient(httpClient, _memoryCache, logger, 1, 3, 2, 10);
        var service = new YahooFinanceService(serviceClient);

        return (service, httpClient);
    }
}

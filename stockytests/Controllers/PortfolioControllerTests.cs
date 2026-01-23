using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using stockyapi.Application.Portfolio;
using stockyapi.Controllers;
using stockyapi.Failures;
using stockyapi.Middleware;
using stockyapi.Requests;
using stockyapi.Responses;
using stockyunittests.Helpers;

namespace stockyunittests.Controllers;

[TestFixture]
public class PortfolioControllerTests
{
    private Mock<IPortfolioApi> _portfolioApi = null!;
    private PortfolioController _controller = null!;
    private static readonly CancellationToken Token = CancellationToken.None;

    [SetUp]
    public void SetUp()
    {
        _portfolioApi = new Mock<IPortfolioApi>();
        _controller = ControllerTestHelpers.SetupController(new PortfolioController(_portfolioApi.Object));
    }

    [Test]
    public async Task ListHoldings_WhenSuccess_ReturnsOk()
    {
        var response = new ListHoldingsResponse(
            100m,
            25m,
            75m,
            ImmutableArray.Create(new HoldingDto
            {
                Ticker = "ABC",
                Quantity = 1m,
                AverageBuyPrice = 10m,
                TotalCost = 10m,
                LastUpdatedTime = DateTime.UtcNow
            }));
        _portfolioApi.Setup(api => api.ListHoldings(Token))
            .ReturnsAsync(Result<ListHoldingsResponse>.Success(response));

        var result = await _controller.ListHoldings(Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _portfolioApi.Verify(api => api.ListHoldings(Token), Times.Once);
    }

    [Test]
    public async Task ListHoldings_WhenFailure_ReturnsProblemDetails()
    {
        var failure = new ServiceUnavailableFailure503();
        _portfolioApi.Setup(api => api.ListHoldings(Token))
            .ReturnsAsync(Result<ListHoldingsResponse>.Fail(failure));

        var result = await _controller.ListHoldings(Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task GetHoldingsById_WhenSuccess_ReturnsOk()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var response = new GetHoldingsResponse(ImmutableArray<HoldingDto>.Empty);
        _portfolioApi.Setup(api => api.GetHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id1, id2 })), Token))
            .ReturnsAsync(Result<GetHoldingsResponse>.Success(response));

        var result = await _controller.GetHoldingsById(new[] { id1.ToString(), id2.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task GetHoldingsById_WhenFailure_ReturnsProblemDetails()
    {
        var id = Guid.NewGuid();
        var failure = new NotFoundFailure404();
        _portfolioApi.Setup(api => api.GetHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id })), Token))
            .ReturnsAsync(Result<GetHoldingsResponse>.Fail(failure));

        var result = await _controller.GetHoldingsById(new[] { id.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task GetHoldingsByTicker_WhenSuccess_ReturnsOk()
    {
        var response = new GetHoldingsResponse(ImmutableArray<HoldingDto>.Empty);
        _portfolioApi.Setup(api => api.GetHoldingsByTicker(
                It.Is<string[]>(symbols => symbols.SequenceEqual(new[] { "ABC", "XYZ" })), Token))
            .ReturnsAsync(Result<GetHoldingsResponse>.Success(response));

        var result = await _controller.GetHoldingsByTicker(new[] { "ABC", "XYZ" }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task GetHoldingsByTicker_WhenFailure_ReturnsProblemDetails()
    {
        var failure = new BadRequestFailure400();
        _portfolioApi.Setup(api => api.GetHoldingsByTicker(
                It.Is<string[]>(symbols => symbols.SequenceEqual(new[] { "ABC" })), Token))
            .ReturnsAsync(Result<GetHoldingsResponse>.Fail(failure));

        var result = await _controller.GetHoldingsByTicker(new[] { "ABC" }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task Buy_WhenSuccess_ReturnsOk()
    {
        var request = new BuyTickerRequest { Symbol = "ABC", Quantity = 1m, Price = 10m };
        var response = new BuyTickerResponse(new TradeConfirmationDto
        {
            Ticker = "ABC",
            QuantityBought = 1m,
            ExecutionPrice = 10m,
            NewAveragePrice = 10m,
            NewCashBalance = 90m,
            NewInvestedAmount = 10m,
            NewTotalValue = 100m,
            TotalCost = 10m,
            TransactionId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow
        });
        _portfolioApi.Setup(api => api.BuyTicker(request, Token))
            .ReturnsAsync(Result<BuyTickerResponse>.Success(response));

        var result = await _controller.Buy(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _portfolioApi.Verify(api => api.BuyTicker(request, Token), Times.Once);
    }

    [Test]
    public async Task Buy_WhenFailure_ReturnsProblemDetails()
    {
        var request = new BuyTickerRequest { Symbol = "ABC", Quantity = 1m, Price = 10m };
        var failure = new ValidationFailure422();
        _portfolioApi.Setup(api => api.BuyTicker(request, Token))
            .ReturnsAsync(Result<BuyTickerResponse>.Fail(failure));

        var result = await _controller.Buy(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task Sell_WhenSuccess_ReturnsOk()
    {
        var request = new SellTickerRequest { Symbol = "ABC", Quantity = 1m, Price = 10m };
        var response = new SellTickerResponse(new TradeConfirmationDto
        {
            Ticker = "ABC",
            QuantityBought = 1m,
            ExecutionPrice = 10m,
            NewAveragePrice = 10m,
            NewCashBalance = 110m,
            NewInvestedAmount = 0m,
            NewTotalValue = 110m,
            TotalCost = 10m,
            TransactionId = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow
        });
        _portfolioApi.Setup(api => api.SellTicker(request, Token))
            .ReturnsAsync(Result<SellTickerResponse>.Success(response));

        var result = await _controller.Sell(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
        _portfolioApi.Verify(api => api.SellTicker(request, Token), Times.Once);
    }

    [Test]
    public async Task Sell_WhenFailure_ReturnsProblemDetails()
    {
        var request = new SellTickerRequest { Symbol = "ABC", Quantity = 1m, Price = 10m };
        var failure = new ConflictFailure409();
        _portfolioApi.Setup(api => api.SellTicker(request, Token))
            .ReturnsAsync(Result<SellTickerResponse>.Fail(failure));

        var result = await _controller.Sell(request, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task DeleteHoldingsById_WhenSuccess_ReturnsOk()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var response = new DeleteHoldingsResponse(new[]
        {
            new DeleteConfirmationDto(id1, "ABC", DateTimeOffset.Now),
            new DeleteConfirmationDto(id2, "XYZ", DateTimeOffset.Now)
        });
        _portfolioApi.Setup(api => api.DeleteHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id1, id2 })), Token))
            .ReturnsAsync(Result<DeleteHoldingsResponse>.Success(response));

        var result = await _controller.DeleteHoldingsById(new[] { id1.ToString(), id2.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task DeleteHoldingsById_WhenFailure_ReturnsProblemDetails()
    {
        var id = Guid.NewGuid();
        var failure = new NotFoundFailure404();
        _portfolioApi.Setup(api => api.DeleteHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id })), Token))
            .ReturnsAsync(Result<DeleteHoldingsResponse>.Fail(failure));

        var result = await _controller.DeleteHoldingsById(new[] { id.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public async Task DeleteHoldingsByTicker_WhenSuccess_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var response = new DeleteHoldingsResponse(new[] { new DeleteConfirmationDto(id, "ABC", DateTimeOffset.Now) });
        _portfolioApi.Setup(api => api.DeleteHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id })), Token))
            .ReturnsAsync(Result<DeleteHoldingsResponse>.Success(response));

        var result = await _controller.DeleteHoldingsByTicker(new[] { id.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
        Assert.That(objectResult.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task DeleteHoldingsByTicker_WhenFailure_ReturnsProblemDetails()
    {
        var id = Guid.NewGuid();
        var failure = new BadRequestFailure400();
        _portfolioApi.Setup(api => api.DeleteHoldingsById(
                It.Is<Guid[]>(ids => ids.SequenceEqual(new[] { id })), Token))
            .ReturnsAsync(Result<DeleteHoldingsResponse>.Fail(failure));

        var result = await _controller.DeleteHoldingsByTicker(new[] { id.ToString() }, Token);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo((int)failure.StatusCode));
        Assert.That(objectResult.Value, Is.TypeOf<ProblemDetails>());
    }

    [Test]
    public void UpdateHoldingsByTicker_ThrowsNotImplemented()
    {
        Assert.ThrowsAsync<NotImplementedException>(() => _controller.UpdateHoldingsByTicker("ABC", Token));
    }
}

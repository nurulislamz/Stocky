using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using stockyapi.Middleware;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class GlobalExceptionHandlerTests
{
    private GlobalExceptionHandler _handler = null!;
    private Mock<IProblemDetailsService> _problemDetailsServiceMock = null!;
    private DefaultHttpContext _httpContext = null!;

    [SetUp]
    public void SetUp()
    {
        _problemDetailsServiceMock = new Mock<IProblemDetailsService>();
        _problemDetailsServiceMock
            .Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Returns(new ValueTask<bool>(true));

        _handler = new GlobalExceptionHandler(
            _problemDetailsServiceMock.Object,
            NullLogger<GlobalExceptionHandler>.Instance);

        _httpContext = new DefaultHttpContext();
        _httpContext.Request.Path = "/api/funds/get";
        _httpContext.TraceIdentifier = "test-trace-id-123";
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_ReturnsTrue()
    {
        var exception = new InvalidOperationException("Something went wrong");

        var result = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_WritesProblemDetailsWithStatus500()
    {
        var exception = new InvalidOperationException("Something went wrong");

        await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Status == StatusCodes.Status500InternalServerError)),
            Times.Once);
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_SetsExpectedTitleAndDetail()
    {
        await _handler.TryHandleAsync(_httpContext, new Exception("Inner message"), CancellationToken.None);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Title == "An unexpected error occurred" &&
                ctx.ProblemDetails.Detail == "An unexpected error occurred")),
            Times.Once);
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_SetsInstanceToRequestPath()
    {
        _httpContext.Request.Path = "/api/portfolio/holdings";

        await _handler.TryHandleAsync(_httpContext, new Exception(), CancellationToken.None);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Instance == "/api/portfolio/holdings")),
            Times.Once);
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_AddsTraceIdAndTimestampToExtensions()
    {
        await _handler.TryHandleAsync(_httpContext, new Exception(), CancellationToken.None);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.Is<ProblemDetailsContext>(ctx =>
                ctx.ProblemDetails.Extensions.ContainsKey("traceId") &&
                Equals(ctx.ProblemDetails.Extensions["traceId"], "test-trace-id-123") &&
                ctx.ProblemDetails.Extensions.ContainsKey("timestamp"))),
            Times.Once);
    }

    [Test]
    public async Task TryHandleAsync_WhenCalled_InvokesProblemDetailsServiceOnce()
    {
        await _handler.TryHandleAsync(_httpContext, new ApplicationException("Test"), CancellationToken.None);

        _problemDetailsServiceMock.Verify(
            s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()),
            Times.Once);
    }
}

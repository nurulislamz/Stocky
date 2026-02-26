using System.Net;
using NUnit.Framework;
using stockyapi.Middleware;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class FailureTests
{
    [TestCase(typeof(BadRequestFailure400), HttpStatusCode.BadRequest, "Bad Request")]
    [TestCase(typeof(UnauthorizedFailure401), HttpStatusCode.Unauthorized, "Unauthorized")]
    [TestCase(typeof(ForbiddenFailure403), HttpStatusCode.Forbidden, "Forbidden")]
    [TestCase(typeof(NotFoundFailure404), HttpStatusCode.NotFound, "Not Found")]
    [TestCase(typeof(ConflictFailure409), HttpStatusCode.Conflict, "Conflict")]
    [TestCase(typeof(ValidationFailure422), HttpStatusCode.UnprocessableEntity, "Validation Failed")]
    [TestCase(typeof(InternalServerFailure500), HttpStatusCode.InternalServerError, "Internal Server Error")]
    [TestCase(typeof(ServiceUnavailableFailure503), HttpStatusCode.ServiceUnavailable, "Service Unavailable")]
    [TestCase(typeof(GatewayTimeoutFailure504), HttpStatusCode.GatewayTimeout, "Gateway Timeout")]
    public void Failure_HasCorrectStatusCodeAndTitle(Type failureType, HttpStatusCode expectedCode, string expectedTitle)
    {
        var failure = (Failure)Activator.CreateInstance(failureType, "Custom detail")!;
        Assert.That(failure.StatusCode, Is.EqualTo(expectedCode));
        Assert.That(failure.Title, Is.EqualTo(expectedTitle));
        Assert.That(failure.Detail, Is.EqualTo("Custom detail"));
    }

    [Test]
    public void None_HasNotImplementedStatusCode()
    {
        var none = new None();
        Assert.That(none.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
        Assert.That(none.Title, Is.EqualTo("No Failure"));
    }

    [Test]
    public void Failure_WithDefaultDetail_UsesDefaultMessage()
    {
        var failure = new NotFoundFailure404();
        Assert.That(failure.Detail, Does.Contain("not found").Or.Contain("Not Found"));
    }
}

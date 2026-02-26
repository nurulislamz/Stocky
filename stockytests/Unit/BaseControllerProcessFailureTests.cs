using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using stockyapi.Controllers.Helpers;
using stockyapi.Middleware;
using stockytests.Helpers;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class BaseControllerProcessFailureTests
{
    /// <summary>
    /// Test controller that exposes ProcessFailure for unit testing.
    /// </summary>
    private sealed class TestController : BaseController
    {
        public ActionResult InvokeProcessFailure(Failure failure) => ProcessFailure(failure);
    }

    [Test]
    public void ProcessFailure_WithNotFound404_Returns404AndProblemDetails()
    {
        var controller = ControllerTestHelpers.SetupController(new TestController());
        var failure = new NotFoundFailure404("Resource not found");

        var result = controller.InvokeProcessFailure(failure);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        var problem = objectResult.Value as ProblemDetails;
        Assert.That(problem, Is.Not.Null);
        Assert.That(problem!.Title, Is.EqualTo(failure.Title));
        Assert.That(problem.Detail, Is.EqualTo(failure.Detail));
    }

    [Test]
    public void ProcessFailure_WithConflict409_Returns409AndProblemDetails()
    {
        var controller = ControllerTestHelpers.SetupController(new TestController());
        var failure = new ConflictFailure409("Insufficient funds");

        var result = controller.InvokeProcessFailure(failure);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        var problem = objectResult.Value as ProblemDetails;
        Assert.That(problem!.Title, Is.EqualTo(failure.Title));
    }

    [Test]
    public void ProcessFailure_WithValidation422_Returns422AndProblemDetails()
    {
        var controller = ControllerTestHelpers.SetupController(new TestController());
        var failure = new ValidationFailure422("Invalid input");

        var result = controller.InvokeProcessFailure(failure);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status422UnprocessableEntity));
        var problem = objectResult.Value as ProblemDetails;
        Assert.That(problem!.Title, Is.EqualTo(failure.Title));
    }

    [Test]
    public void ProcessFailure_WithInternalServerError500_Returns500()
    {
        var controller = ControllerTestHelpers.SetupController(new TestController());
        var failure = new InternalServerFailure500("Server error");

        var result = controller.InvokeProcessFailure(failure);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
    }
}

using NUnit.Framework;
using stockyapi.Middleware;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class ResultTests
{
    [Test]
    public void Success_SetsIsSuccessTrue_AndValue()
    {
        var value = 42;
        var result = Result<int>.Success(value);
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.IsFailure, Is.False);
        Assert.That(result.Value, Is.EqualTo(42));
    }

    [Test]
    public void Fail_SetsIsSuccessFalse_AndFailure()
    {
        var failure = new BadRequestFailure400("Invalid");
        var result = Result<string>.Fail(failure);
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Failure, Is.SameAs(failure));
    }

    [Test]
    public void ImplicitConversion_FromValue_ProducesSuccess()
    {
        Result<int> result = 10;
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(10));
    }

    [Test]
    public void ImplicitConversion_FromFailure_ProducesFailure()
    {
        var failure = new NotFoundFailure404();
        Result<int> result = failure;
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Failure, Is.SameAs(failure));
    }

    [Test]
    public void Success_WithReferenceType_StoresReference()
    {
        var obj = new object();
        var result = Result<object>.Success(obj);
        Assert.That(result.Value, Is.SameAs(obj));
    }

    [Test]
    public void Fail_WithNone_IsStillFailure()
    {
        var result = Result<int>.Fail(new None());
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Failure, Is.InstanceOf<None>());
    }
}

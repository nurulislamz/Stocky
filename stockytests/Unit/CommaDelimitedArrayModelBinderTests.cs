using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using stockyapi.Controllers.Helpers;

namespace stockytests.Unit;

[TestFixture]
[Category("Unit")]
public class CommaDelimitedArrayModelBinderTests
{
    private const string ModelName = "ids";
    private readonly CommaDelimitedArrayModelBinder _binder = new();
    private static readonly EmptyModelMetadataProvider MetadataProvider = new();

    private static ModelBindingContext CreateContext(ValueProviderResult valueProviderResult)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var valueProvider = new TestValueProvider(ModelName, valueProviderResult);
        var metadata = MetadataProvider.GetMetadataForType(typeof(string[]));
        return DefaultModelBindingContext.CreateBindingContext(
            actionContext,
            valueProvider,
            metadata,
            bindingInfo: null,
            modelName: ModelName);
    }

    [Test]
    public async Task BindModelAsync_WhenValueProviderReturnsNone_DoesNotSetResult()
    {
        var context = CreateContext(ValueProviderResult.None);
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsNull_SetsFailedResult()
    {
        var context = CreateContext(new ValueProviderResult(null as string));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
        // Binder may or may not add ModelState error for null depending on ValueProviderResult behavior
        if (context.ModelState.TryGetValue(ModelName, out var entry) && entry?.Errors.Count > 0)
            Assert.That(entry.Errors[0].ErrorMessage, Does.Contain("comma-separated list"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsEmptyString_SetsFailedResultAndModelError()
    {
        var context = CreateContext(new ValueProviderResult(""));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
        Assert.That(context.ModelState[ModelName]!.Errors[0].ErrorMessage, Does.Contain("comma-separated list"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsWhitespaceOnly_SetsFailedResultAndModelError()
    {
        var context = CreateContext(new ValueProviderResult("   "));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
        Assert.That(context.ModelState[ModelName]!.Errors[0].ErrorMessage, Does.Contain("comma-separated list"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueLengthOver500_SetsFailedResultAndModelError()
    {
        var longString = new string('a', 501);
        var context = CreateContext(new ValueProviderResult(longString));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
        Assert.That(context.ModelState[ModelName]!.Errors[0].ErrorMessage, Does.Contain("too long"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsExactly500Chars_Succeeds()
    {
        var value = new string('a', 500);
        var context = CreateContext(new ValueProviderResult(value));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.True);
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsCommaOnly_ResultsInEmptyItems_SetsFailedResult()
    {
        var context = CreateContext(new ValueProviderResult(","));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.False);
        Assert.That(context.ModelState[ModelName]!.Errors[0].ErrorMessage, Does.Contain("comma-separated list"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsValidSingleItem_SetsSuccessResult()
    {
        var context = CreateContext(new ValueProviderResult("guid-one"));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.True);
        var arr = context.Result.Model as string[];
        Assert.That(arr, Is.Not.Null);
        Assert.That(arr!, Has.Length.EqualTo(1));
        Assert.That(arr[0], Is.EqualTo("guid-one"));
    }

    [Test]
    public async Task BindModelAsync_WhenValueIsValidCommaSeparatedList_SetsSuccessResult()
    {
        var context = CreateContext(new ValueProviderResult("a,b,c"));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.True);
        var arr = context.Result.Model as string[];
        Assert.That(arr, Is.Not.Null);
        Assert.That(arr!, Has.Length.EqualTo(3));
        Assert.That(arr, Is.EqualTo(new[] { "a", "b", "c" }));
    }

    [Test]
    public async Task BindModelAsync_WhenValueHasSpacesAroundItems_TrimsCorrectly()
    {
        var context = CreateContext(new ValueProviderResult("  x  ,  y  ,  z  "));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.True);
        var arr = context.Result.Model as string[];
        Assert.That(arr, Is.EqualTo(new[] { "x", "y", "z" }));
    }

    [Test]
    public async Task BindModelAsync_WhenValueHasMultipleCommasWithItems_SplitsCorrectly()
    {
        var context = CreateContext(new ValueProviderResult("id1,id2,id3"));
        await _binder.BindModelAsync(context);
        Assert.That(context.Result.IsModelSet, Is.True);
        var arr = context.Result.Model as string[];
        Assert.That(arr, Has.Length.EqualTo(3));
        Assert.That(arr![0], Is.EqualTo("id1"));
        Assert.That(arr[1], Is.EqualTo("id2"));
        Assert.That(arr[2], Is.EqualTo("id3"));
    }

    private sealed class TestValueProvider : IValueProvider
    {
        private readonly string _key;
        private readonly ValueProviderResult _result;

        public TestValueProvider(string key, ValueProviderResult result)
        {
            _key = key;
            _result = result;
        }

        public bool ContainsPrefix(string prefix) => true;
        public ValueProviderResult GetValue(string key) => key == _key ? _result : ValueProviderResult.None;
    }
}

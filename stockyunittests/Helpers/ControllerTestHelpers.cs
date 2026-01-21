using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace stockyunittests.Helpers;

public static class ControllerTestHelpers
{
    public static TController SetupController<TController>(TController controller) where TController : ControllerBase
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/test";

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ProblemDetailsFactory>(
            new DefaultProblemDetailsFactory(Options.Create(new ApiBehaviorOptions())));
        httpContext.RequestServices = services.BuildServiceProvider();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }
}

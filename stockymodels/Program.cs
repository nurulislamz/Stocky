using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using stockymodels.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.Use(hostContext.Configuration.GetConnectionString("DefaultConnection")));
    });

var host = builder.Build();
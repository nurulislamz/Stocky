using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using stockymodels.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // SqlLite Used for developement
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
        
        // PostgresSql
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
    });

var host = builder.Build();
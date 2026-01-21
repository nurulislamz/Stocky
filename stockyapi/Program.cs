using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using stockyapi.Services;
using stockyapi.Options;
using stockyapi.Extensions;
using stockymodels.Data;
using System.Text.Json.Serialization;
using stockyapi.Controllers;
using stockyapi.Middleware;
using stockyapi.Repository.Portfolio;
using stockyapi.Repository.User;
using stockyapi.Repository.YahooFinance;
using Polly;
using stockyapi.Services.YahooFinance;

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        var logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("Startup");
        var cs = builder.Configuration.GetConnectionString("DefaultConnection");
        logger.LogInformation("ConnectionString={cs}", cs);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline
        ConfigureMiddleware(app, app.Environment);

        app.Run();
    }

    private static void ConfigureMiddleware(IApplicationBuilder app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwaggerDocumentation();
        }

        app.UseRouting();
        app.UseCors("AllowLocalReactApp");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerDocumentation();

        // TODO: Add SqlLite 
        
        // Configure PostgreSQL with retry policy
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            });
        });

        // Registers a request-scoped user context abstraction.
        // This allows application handlers to access the current authenticated user
        // (e.g. UserId) without directly depending on HttpContext or ASP.NET.
        // HttpUserContext is the web-layer implementation that reads claims from HttpContext.
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, HttpUserContext>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        // Service DIs
        services.AddScoped<ITokenService, TokenService>();
        services.AddHttpClient<IYahooFinanceService, YahooFinanceService>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        // Repository DIs
        services.AddScoped<IFundsRepository, FundsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddMemoryCache();

        // Authentication Services
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
            };
        });

        // Local Dev CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalReactApp",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }
}
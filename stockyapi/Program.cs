using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using stockyapi.Extensions;
using stockyapi.Middleware;
using stockyapi.Options;
using stockyapi.Repository.Funds;
using stockyapi.Repository.PortfolioRepository;
using stockyapi.Repository.User;
using stockyapi.Services;
using stockyapi.Services.YahooFinance;
using stockyapi.Application.Auth;
using stockyapi.Application.Funds;
using stockyapi.Application.MarketPricing;
using stockyapi.Application.Portfolio;
using stockyapi.Controllers.Helpers;
using stockyapi.Services.YahooFinance.Helper;
using stockymodels.Data;

namespace stockyapi;

internal class Program
{
    private static readonly ILogger Logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger("Startup");
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
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
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerDocumentation();

        // TODO: Add SqlLite 
        
        // Configure database provider (PostgreSQL in production, SQLite when Dev flag is set)
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var providerConnection = configuration.GetValue<bool>("Dev")
                ? configuration.GetConnectionString("SqliteConnection")
                : configuration.GetConnectionString("DefaultConnection");

            if (configuration.GetValue<bool>("Dev"))
            {
                Logger.LogInformation("Using SQLite connection string {Connection}", providerConnection);
                options.UseSqlite(providerConnection);
                return;
            }

            Logger.LogInformation("Using Postgres connection string {Connection}", providerConnection);
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
        services.AddScoped<IYahooFinanceService, YahooFinanceService>();
        services.AddHttpClient<YahooApiServiceClient>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", UserAgents.GetRandomNewUserAgent());
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        
        
        // Application DIs
        services.AddScoped<IAuthenticationApi, AuthenticationApi>();
        services.AddScoped<IFundsApi, FundsApi>();
        services.AddScoped<IMarketPricingApi, MarketPricingApi>();
        services.AddScoped<IPortfolioApi, PortfolioApi>();

        // Repository DIs
        services.AddScoped<IFundsRepository, FundsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException()))
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
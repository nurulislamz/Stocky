using Microsoft.EntityFrameworkCore;
using stockymodels.models;
using stockymodels.Data.Configurations;

namespace stockymodels.Data;

public class ApplicationDbContext : DbContext
{
  public DbSet<UserModel> Users { get; set; }
  public DbSet<PortfolioModel> Portfolios { get; set; }
  public DbSet<StockHoldingModel> StockHoldings { get; set; }
  public DbSet<TransactionModel> Transactions { get; set; }
  public DbSet<WatchlistModel> Watchlist { get; set; }
  public DbSet<UserPreferencesModel> UserPreferences { get; set; }
  public DbSet<PriceAlertModel> PriceAlerts { get; set; }

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configure all entities that inherit from BaseModel
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
      {
        modelBuilder.Entity(entityType.ClrType)
          .Property("CreatedAt")
          .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity(entityType.ClrType)
          .Property("UpdatedAt")
          .HasDefaultValueSql("CURRENT_TIMESTAMP")
          .ValueGeneratedOnAddOrUpdate();
      }
    }
  }
}
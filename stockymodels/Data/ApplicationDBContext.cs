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

    // Apply all configurations
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    // Configure timestamps for all BaseModel entities
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
      {
        modelBuilder.Entity(entityType.ClrType)
          .Property("CreatedAt")
          .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity(entityType.ClrType)
          .Property("UpdatedAt")
          .HasDefaultValueSql("GETUTCDATE()")
          .ValueGeneratedOnAddOrUpdate();
      }
    }
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    UpdateTimestamps();
    return base.SaveChangesAsync(cancellationToken);
  }

  private void UpdateTimestamps()
  {
    var entries = ChangeTracker.Entries<BaseModel>();
    foreach (var entry in entries)
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedAt = DateTime.UtcNow;
        entry.Entity.UpdatedAt = DateTime.UtcNow;
      }
      else if (entry.State == EntityState.Modified)
      {
        entry.Entity.UpdatedAt = DateTime.UtcNow;
      }
    }
  }
}
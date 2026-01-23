using Microsoft.EntityFrameworkCore;
using stockymodels.models;


namespace stockymodels.Data;

public class ApplicationDbContext : DbContext
{
  public DbSet<UserModel> Users { get; set; } = null!;
  public DbSet<PortfolioModel> Portfolios { get; set; } = null!;
  public DbSet<StockHoldingModel> StockHoldings { get; set; } = null!;
  public DbSet<AssetTransactionModel> AssetTransactions { get; set; } = null!;
  public DbSet<FundsTransactionModel> FundsTransactions { get; set; } = null!;
  public DbSet<WatchlistModel> Watchlist { get; set; } = null!;
  public DbSet<UserPreferencesModel> UserPreferences { get; set; } = null!;
  public DbSet<PriceAlertModel> PriceAlerts { get; set; } = null!;

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public override int SaveChanges()
  {
    ApplyTimestamps();
    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    ApplyTimestamps();
    return base.SaveChangesAsync(cancellationToken);
  }

  private void ApplyTimestamps()
  {
    var utcNow = DateTime.UtcNow;

    foreach (var entry in ChangeTracker.Entries<BaseModel>())
    {
      if (entry.State == EntityState.Added)
      {
        entry.Entity.CreatedAt = utcNow;
        entry.Entity.UpdatedAt = utcNow;
      }
      else if (entry.State == EntityState.Modified)
      {
        entry.Entity.UpdatedAt = utcNow;
      }
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

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
          .HasDefaultValueSql("CURRENT_TIMESTAMP");
      }
    }
  }
}

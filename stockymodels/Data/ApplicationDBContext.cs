using Microsoft.EntityFrameworkCore;
using stockymodels.models;


namespace stockymodels.Data;

[Obsolete]
public class ApplicationDbContext : DbContext
{
  public DbSet<UserAggregate> Users { get; set; } = null!;
  public DbSet<PortfolioAggregate> Portfolios { get; set; } = null!;
  public DbSet<StockHoldingAggregate> StockHoldings { get; set; } = null!;
  public DbSet<EventAggregate> EventModels { get; set; } = null!;
  public DbSet<CommandAggregate> Commands { get; set; } = null!;
  public DbSet<WatchlistAggregate> Watchlist { get; set; } = null!;
  public DbSet<UserPreferencesAggregate> UserPreferences { get; set; } = null!;
  public DbSet<PriceAlertAggregate> PriceAlerts { get; set; } = null!;

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public override int SaveChanges()
  {
    ApplyTimestamps();
    EnforceEventStoreAppendOnly();
    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    ApplyTimestamps();
    EnforceEventStoreAppendOnly();
    return base.SaveChangesAsync(cancellationToken);
  }

  /// <summary>Events table is append-only. Reject any update or delete of EventAggregate.</summary>
  private void EnforceEventStoreAppendOnly()
  {
    foreach (var entry in ChangeTracker.Entries<EventAggregate>())
    {
      if (entry.State is EntityState.Modified or EntityState.Deleted)
        throw new InvalidOperationException(
          "EventAggregate is append-only; updates and deletes are not allowed.");
    }
  }

  private void ApplyTimestamps()
  {
    var utcNow = DateTime.UtcNow;

    foreach (var entry in ChangeTracker.Entries<BaseAggregate>())
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

    // Configure all entities that inherit from BaseAggregate
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(BaseAggregate).IsAssignableFrom(entityType.ClrType))
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

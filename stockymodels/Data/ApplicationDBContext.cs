using Microsoft.EntityFrameworkCore;
using stockymodels.models;

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

    // Configure relationships
    modelBuilder.Entity<UserModel>()
      .HasOne(u => u.Portfolio)
      .WithOne(p => p.User)
      .HasForeignKey<PortfolioModel>(p => p.UserId);

    modelBuilder.Entity<UserModel>()
      .HasOne(u => u.Preferences)
      .WithOne(p => p.User)
      .HasForeignKey<UserPreferencesModel>(p => p.UserId);

    modelBuilder.Entity<UserModel>()
      .HasMany(u => u.Watchlist)
      .WithOne(w => w.User)
      .HasForeignKey(w => w.UserId);

    modelBuilder.Entity<UserModel>()
      .HasMany(u => u.PriceAlerts)
      .WithOne(p => p.User)
      .HasForeignKey(p => p.UserId);
  }
}
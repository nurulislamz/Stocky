using Microsoft.EntityFrameworkCore;
using stockymodels.models;
using stockymodels.Models;

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

    // User-Portfolio relationship (One-to-One)
    modelBuilder.Entity<UserModel>()
        .HasOne(u => u.Portfolio)
        .WithOne(p => p.User)
        .HasForeignKey<PortfolioModel>(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // User-Preferences relationship (One-to-One)
    modelBuilder.Entity<UserModel>()
        .HasOne(u => u.Preferences)
        .WithOne(p => p.User)
        .HasForeignKey<UserPreferencesModel>(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // User-Watchlist relationship (One-to-Many)
    modelBuilder.Entity<UserModel>()
        .HasMany(u => u.Watchlist)
        .WithOne(w => w.User)
        .HasForeignKey(w => w.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // User-PriceAlerts relationship (One-to-Many)
    modelBuilder.Entity<UserModel>()
        .HasMany(u => u.PriceAlerts)
        .WithOne(p => p.User)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    // Portfolio-StockHoldings relationship (One-to-Many)
    modelBuilder.Entity<PortfolioModel>()
        .HasMany(p => p.StockHoldings)
        .WithOne(s => s.Portfolio)
        .HasForeignKey(s => s.PortfolioId)
        .OnDelete(DeleteBehavior.Cascade);

    // Portfolio-Transactions relationship (One-to-Many)
    modelBuilder.Entity<PortfolioModel>()
        .HasMany(p => p.Transactions)
        .WithOne(t => t.Portfolio)
        .HasForeignKey(t => t.PortfolioId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}
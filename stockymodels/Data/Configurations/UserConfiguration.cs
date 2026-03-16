using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class UserConfiguration : IEntityTypeConfiguration<UserAggregate>
{
    public void Configure(EntityTypeBuilder<UserAggregate> builder)
    {
        builder.ToTable("Users", "stockydb");

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.HasOne(u => u.Portfolio)
            .WithOne(p => p.User)
            .HasForeignKey<PortfolioAggregate>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Preferences)
            .WithOne(p => p.User)
            .HasForeignKey<UserPreferencesAggregate>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Watchlist)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.PriceAlerts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
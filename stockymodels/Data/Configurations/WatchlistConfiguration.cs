using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class WatchlistConfiguration : IEntityTypeConfiguration<WatchlistAggregate>
{
    public void Configure(EntityTypeBuilder<WatchlistAggregate> builder)
    {
        builder.ToTable("Watchlist", "stockydb");

        builder.Property(w => w.Notes)
            .HasMaxLength(500);

        builder.HasIndex(e => new { e.UserId, e.Symbol })
            .IsUnique();
    }
}

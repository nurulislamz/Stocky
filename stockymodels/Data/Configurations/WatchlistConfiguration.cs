using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class WatchlistConfiguration : IEntityTypeConfiguration<WatchlistModel>
{
    public void Configure(EntityTypeBuilder<WatchlistModel> builder)
    {
        builder.ToTable("Watchlist");

        builder.Property(w => w.Notes)
            .HasMaxLength(500);

        builder.HasIndex(e => new { e.UserId, e.Symbol })
            .IsUnique();
    }
}

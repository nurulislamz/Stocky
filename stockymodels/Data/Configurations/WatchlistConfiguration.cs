using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class WatchlistConfiguration : IEntityTypeConfiguration<WatchlistModel>
{
    public void Configure(EntityTypeBuilder<WatchlistModel> builder)
    {
        builder.ToTable("Watchlist");

        builder.HasIndex(e => new { e.UserId, e.Symbol })
            .IsUnique();
    }
}
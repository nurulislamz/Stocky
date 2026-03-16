using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class PriceAlertConfiguration : IEntityTypeConfiguration<PriceAlertAggregate>
{
    public void Configure(EntityTypeBuilder<PriceAlertAggregate> builder)
    {
        builder.ToTable("PriceAlerts", "stockydb");

        builder.Property(p => p.Condition)
            .HasMaxLength(10);

        builder.HasIndex(e => new { e.Symbol, e.IsTriggered });
        builder.HasIndex(e => new { e.UserId, e.Symbol })
            .IsUnique();
    }
}

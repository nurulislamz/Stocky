using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class PriceAlertConfiguration : IEntityTypeConfiguration<PriceAlertModel>
{
    public void Configure(EntityTypeBuilder<PriceAlertModel> builder)
    {
        builder.ToTable("PriceAlerts");

        builder.HasIndex(e => new { e.Symbol, e.IsTriggered });
        builder.HasIndex(e => new { e.UserId, e.Symbol })
            .IsUnique();
    }
}
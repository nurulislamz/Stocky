using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class StockHoldingConfiguration : IEntityTypeConfiguration<StockHoldingAggregate>
{
    public void Configure(EntityTypeBuilder<StockHoldingAggregate> builder)
    {
        builder.ToTable("StockHoldings", "stockydb");

        builder.Property(e => e.Ticker)
            .HasMaxLength(20);

        builder.HasIndex(e => e.Ticker);
        builder.HasIndex(e => new { e.PortfolioId, Symbol = e.Ticker })
            .IsUnique();
    }
}

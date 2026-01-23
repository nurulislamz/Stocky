using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class StockHoldingConfiguration : IEntityTypeConfiguration<StockHoldingModel>
{
    public void Configure(EntityTypeBuilder<StockHoldingModel> builder)
    {
        builder.ToTable("StockHoldings");

        builder.Property(e => e.Ticker)
            .HasMaxLength(20);

        builder.HasIndex(e => e.Ticker);
        builder.HasIndex(e => new { e.PortfolioId, Symbol = e.Ticker })
            .IsUnique();
    }
}

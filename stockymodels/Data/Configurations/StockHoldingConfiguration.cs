using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class StockHoldingConfiguration : IEntityTypeConfiguration<StockHoldingModel>
{
    public void Configure(EntityTypeBuilder<StockHoldingModel> builder)
    {
        builder.ToTable("StockHoldings");

        builder.HasIndex(e => e.Symbol);
        builder.HasIndex(e => new { e.PortfolioId, e.Symbol })
            .IsUnique();
    }
}
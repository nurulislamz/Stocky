using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class PortfolioConfiguration : IEntityTypeConfiguration<PortfolioAggregate>
{
    public void Configure(EntityTypeBuilder<PortfolioAggregate> builder)
    {
        builder.ToTable("Portfolios", "stockydb");

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.HasMany(p => p.StockHoldings)
            .WithOne(s => s.Portfolio)
            .HasForeignKey(s => s.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

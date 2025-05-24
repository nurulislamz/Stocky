using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class PortfolioConfiguration : IEntityTypeConfiguration<PortfolioModel>
{
    public void Configure(EntityTypeBuilder<PortfolioModel> builder)
    {
        builder.ToTable("Portfolios");

        builder.HasIndex(e => new { e.UserId, e.Id })
            .IsUnique();

        builder.HasMany(p => p.StockHoldings)
            .WithOne(s => s.Portfolio)
            .HasForeignKey(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Transactions)
            .WithOne(t => t.Portfolio)
            .HasForeignKey(t => t.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
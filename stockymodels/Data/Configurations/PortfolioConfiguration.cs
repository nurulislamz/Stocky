using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class PortfolioConfiguration : IEntityTypeConfiguration<PortfolioModel>
{
    public void Configure(EntityTypeBuilder<PortfolioModel> builder)
    {
        builder.ToTable("Portfolios");

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.HasMany(p => p.StockHoldings)
            .WithOne(s => s.Portfolio)
            .HasForeignKey(s => s.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Transactions)
            .WithOne(t => t.Portfolio)
            .HasForeignKey(t => t.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Funds)
            .WithOne(f => f.Portfolio)
            .HasForeignKey(f => f.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<AssetTransactionModel>
{
    public void Configure(EntityTypeBuilder<AssetTransactionModel> builder)
    {
        builder.ToTable("Transactions");

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.PortfolioId, e.CreatedAt });
    }
}
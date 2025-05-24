using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionModel>
{
    public void Configure(EntityTypeBuilder<TransactionModel> builder)
    {
        builder.ToTable("Transactions");

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => new { e.PortfolioId, e.CreatedAt });
    }
}
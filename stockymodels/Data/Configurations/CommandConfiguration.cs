using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class CommandConfiguration : IEntityTypeConfiguration<CommandAggregate>
{
    public void Configure(EntityTypeBuilder<CommandAggregate> builder)
    {
        builder.ToTable("Commands", "stockydb");

        builder.HasKey(c => c.CommandId);
        builder.Property(c => c.CommandId).HasColumnName("CommandId");

        builder.Property(c => c.CommandType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(c => c.CommandPayloadJson)
            .IsRequired();

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.RequestId);
    }
}

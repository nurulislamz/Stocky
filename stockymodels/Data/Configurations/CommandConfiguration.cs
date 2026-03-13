using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class CommandConfiguration : IEntityTypeConfiguration<CommandModel>
{
    public void Configure(EntityTypeBuilder<CommandModel> builder)
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

        builder.HasMany(c => c.Events)
            .WithOne(e => e.Command)
            .HasForeignKey(e => e.CommandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class CommandConfiguration : IEntityTypeConfiguration<CommandModel>
{
    public void Configure(EntityTypeBuilder<CommandModel> builder)
    {
        builder.ToTable("Commands");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("CommandId");

        builder.Property(c => c.CommandType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(c => c.CommandPayloadJson)
            .IsRequired();

        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.RequestId);
        builder.HasIndex(c => c.IssuedAt);

        builder.HasMany(c => c.Events)
            .WithOne()
            .HasForeignKey(e => e.CommandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

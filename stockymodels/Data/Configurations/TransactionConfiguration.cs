using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventModel>
{
    public void Configure(EntityTypeBuilder<EventModel> builder)
    {
        builder.ToTable("Events", "stockydb");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("EventId").ValueGeneratedOnAdd();

        builder.Property(e => e.EventPayloadJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.AggregateType, e.AggregateId, e.SequenceId }).IsUnique();

        builder.HasOne(e => e.Command)
            .WithMany(c => c.Events)
            .HasForeignKey(e => e.CommandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventAggregate>
{
    public void Configure(EntityTypeBuilder<EventAggregate> builder)
    {
        builder.ToTable("Events", "stockydb");

        builder.HasKey(e => e.EventId);
        builder.Property(e => e.EventId).HasColumnName("EventId").ValueGeneratedOnAdd();

        builder.Property(e => e.EventPayloadJson)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.AggregateType, e.AggregateId, e.AggregateSequenceId }).IsUnique();
    }
}
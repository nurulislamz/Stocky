using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventModel>
{
    public void Configure(EntityTypeBuilder<EventModel> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("EventId");

        builder.Property(e => e.EventPayloadProtobuf)
            .IsRequired();
        builder.Property(e => e.EventPayloadJson)
            .IsRequired();
    }
}
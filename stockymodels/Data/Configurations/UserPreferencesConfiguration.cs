using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

namespace stockymodels.Data.Configurations;

[Obsolete]
public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferencesAggregate>
{
    public void Configure(EntityTypeBuilder<UserPreferencesAggregate> builder)
    {
        builder.ToTable("UserPreferences", "stockydb");

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}

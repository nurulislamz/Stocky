using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using stockymodels.models;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferencesModel>
{
    public void Configure(EntityTypeBuilder<UserPreferencesModel> builder)
    {
        builder.ToTable("UserPreferences");

        builder.HasIndex(e => e.UserId)
            .IsUnique();
    }
}
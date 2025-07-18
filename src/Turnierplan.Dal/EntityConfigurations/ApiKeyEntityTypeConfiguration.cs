using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ApiKey;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApiKeyEntityTypeConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PrincipalId)
            .IsRequired();

        builder.HasIndex(x => x.PrincipalId)
            .IsUnique();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.HasMany(x => x.RoleAssignments)
            .WithOne(x => x.Scope)
            .HasForeignKey("ApiKeyId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.SecretHash)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ExpiryDate)
            .IsRequired();

        builder.Ignore(x => x.IsExpired);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Requests)
            .WithOne(x => x.ApiKey)
            .HasForeignKey("ApiKeyId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(ApiKey.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(ApiKey.Requests))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

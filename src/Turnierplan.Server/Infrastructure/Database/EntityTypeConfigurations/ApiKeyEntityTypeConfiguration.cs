using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Domain.ApiKeys;
using Turnierplan.Server.Infrastructure.Database.Converters;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class ApiKeyEntityTypeConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("api_key", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.PrincipalId)
            .IsRequired();

        builder.HasIndex(x => x.PrincipalId)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.SecretHash)
            .IsRequired();

        builder.Property(x => x.ExpiryDate)
            .IsRequired();

        builder.Ignore(x => x.IsExpired);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Requests)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

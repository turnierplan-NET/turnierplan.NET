using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Infrastructure.Database.Converters;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class OrganizationEntityTypeConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organization", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(x => x.Projects)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

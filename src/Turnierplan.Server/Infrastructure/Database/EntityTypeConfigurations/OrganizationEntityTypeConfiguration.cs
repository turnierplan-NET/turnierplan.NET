using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Infrastructure.Database.Converters;
using Turnierplan.Server.Infrastructure.Database.Entities;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class OrganizationEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationEntity>
{
    public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
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
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Domain.Projects;
using Turnierplan.Server.Infrastructure.Database.Converters;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("project", TurnierplanContext.Schema);

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

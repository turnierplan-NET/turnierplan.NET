using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.EntityConfigurations;

internal sealed class ResourceEntityTypeConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Notes);

        builder.HasMany<ResourceAssignment>()
            .WithOne(x => x.Resource)
            .HasForeignKey("ResourceId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

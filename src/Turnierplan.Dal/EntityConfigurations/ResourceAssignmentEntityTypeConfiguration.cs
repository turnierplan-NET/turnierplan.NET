using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.EntityConfigurations;

internal sealed class ResourceAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<ResourceAssignment>
{
    public void Configure(EntityTypeBuilder<ResourceAssignment> builder)
    {
        builder.ToTable("ResourceAssignments", TurnierplanContext.Schema);

        builder.HasKey("ResourceGroupId", "ResourceId");

        builder.Property(x => x.State)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.EntityConfigurations;

internal sealed class ResourceGroupEntityTypeConfiguration : IEntityTypeConfiguration<ResourceGroup>
{
    public void Configure(EntityTypeBuilder<ResourceGroup> builder)
    {
        builder.ToTable("ResourceGroups", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name);

        builder.Property(x => x.Description);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Start);

        builder.Property(x => x.End);

        builder.HasMany(x => x.ResourceAssignments)
            .WithOne(x => x.ResourceGroup)
            .HasForeignKey("ResourceGroupId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(ResourceGroup.ResourceAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

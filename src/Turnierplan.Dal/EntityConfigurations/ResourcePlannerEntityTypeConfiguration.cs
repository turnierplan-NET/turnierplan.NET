using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ResourcePlannerEntityTypeConfiguration : IEntityTypeConfiguration<ResourcePlanner>
{
    public void Configure(EntityTypeBuilder<ResourcePlanner> builder)
    {
        builder.ToTable("ResourcePlanners", TurnierplanContext.Schema);

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

        builder.HasMany(x => x.RoleAssignments)
            .WithOne(x => x.Scope)
            .HasForeignKey("ResourcePlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.ResourceGroups)
            .WithOne(x => x.ResourcePlanner)
            .HasForeignKey("ResourcePlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.ResourcePlannerViews)
            .WithOne(x => x.ResourcePlanner)
            .HasForeignKey("ResourcePlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(ResourcePlanner.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(ResourcePlanner.ResourceGroups))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(ResourcePlanner.ResourcePlannerViews))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

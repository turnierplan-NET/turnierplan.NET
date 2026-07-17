using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;

namespace Turnierplan.Dal.EntityConfigurations;

internal sealed class ResourcePlannerViewEntityTypeConfiguration : IEntityTypeConfiguration<ResourcePlannerView>
{
    public void Configure(EntityTypeBuilder<ResourcePlannerView> builder)
    {
        builder.ToTable("ResourcePlannerViews", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasMany(x => x.ResourceGroups)
            .WithMany();

        builder.Metadata.FindNavigation(nameof(ResourcePlannerView.ResourceGroups))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

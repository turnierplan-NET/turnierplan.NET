using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.ResourcePlanner;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

internal sealed class ResourcePlannerViewEntityTypeConfiguration : IEntityTypeConfiguration<ResourcePlannerView>
{
    public void Configure(EntityTypeBuilder<ResourcePlannerView> builder)
    {
        builder.ToTable("ResourcePlannerViews", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.DisplayAllGroups)
            .IsRequired();

        builder.HasMany(x => x.ResourceGroups)
            .WithMany();
    }
}

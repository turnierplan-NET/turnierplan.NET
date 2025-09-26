using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApplicationTeamLabelEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationTeamLabel>
{
    public void Configure(EntityTypeBuilder<ApplicationTeamLabel> builder)
    {
        builder.ToTable("ApplicationTeamLabels", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.IsSystemCreated)
            .IsRequired();

        builder.Property(x => x.ColorCode)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class LabelEntityTypeConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels", TurnierplanContext.Schema);

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

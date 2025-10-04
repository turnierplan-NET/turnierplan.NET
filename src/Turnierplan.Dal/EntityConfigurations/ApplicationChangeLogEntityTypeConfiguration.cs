using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApplicationChangeLogEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationChangeLog>
{
    public void Configure(EntityTypeBuilder<ApplicationChangeLog> builder)
    {
        builder.ToTable("ApplicationChangeLogs", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.OldValue)
            .IsRequired(false);

        builder.Property(x => x.NewValue)
            .IsRequired(false);
    }
}

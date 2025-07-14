using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TournamentClassEntityTypeConfiguration : IEntityTypeConfiguration<TournamentClass>
{
    public void Configure(EntityTypeBuilder<TournamentClass> builder)
    {
        builder.ToTable("TournamentClasses", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.MaxTeamCount);
    }
}

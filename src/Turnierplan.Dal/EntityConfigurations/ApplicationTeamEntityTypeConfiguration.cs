using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApplicationTeamEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationTeam>
{
    public void Configure(EntityTypeBuilder<ApplicationTeam> builder)
    {
        builder.ToTable("ApplicationTeams", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasOne<TournamentClass>(x => x.Class)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();
    }
}

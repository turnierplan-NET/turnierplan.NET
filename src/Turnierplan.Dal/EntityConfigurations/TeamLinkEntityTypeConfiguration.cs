using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TeamLinkEntityTypeConfiguration : IEntityTypeConfiguration<TeamLink>
{
    public void Configure(EntityTypeBuilder<TeamLink> builder)
    {
        builder.ToTable("TeamLinks", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.ApplicationTeam)
            .WithOne(x => x.TeamLink)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.Team)
            .WithOne(x => x.TeamLink)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

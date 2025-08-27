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

        builder.HasOne(x => x.PlanningRealm)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.ApplicationTeam)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.Tournament)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(x => x.Team)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

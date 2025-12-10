using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TeamEntityTypeConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams", TurnierplanContext.Schema);

        builder.HasKey("TournamentId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OutOfCompetition)
            .IsRequired();

        builder.Property(x => x.EntryFeePaidAt);

        builder.Ignore(x => x.Statistics);

        builder.HasMany<GroupParticipant>()
            .WithOne(x => x.Team)
            .HasForeignKey("TournamentId", "TeamId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

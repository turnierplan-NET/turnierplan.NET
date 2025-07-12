using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("Matches", TurnierplanContext.Schema);

        builder.HasKey("TournamentId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Index)
            .IsRequired();

        builder.Property(x => x.Court)
            .IsRequired();

        builder.Property(x => x.Kickoff);

        builder.Property(x => x.TeamSelectorA)
            .HasConversion(new TeamSelectorConverter())
            .IsRequired();

        builder.Property(x => x.TeamSelectorB)
            .HasConversion(new TeamSelectorConverter())
            .IsRequired();

        builder.Ignore(x => x.TeamA);

        builder.Ignore(x => x.TeamB);

        builder.Property(x => x.OutcomeType);

        builder.Property(x => x.FinalsRound);

        builder.Property(x => x.PlayoffPosition);

        builder.Property(x => x.IsCurrentlyPlaying);

        builder.Property(x => x.ScoreA);

        builder.Property(x => x.ScoreB);

        builder.Ignore(x => x.IsGroupMatch);

        builder.Ignore(x => x.IsDecidingMatch);

        builder.Ignore(x => x.IsFinished);
    }
}

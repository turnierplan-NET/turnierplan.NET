using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class RankingOverwriteEntityTypeConfiguration : IEntityTypeConfiguration<RankingOverwrite>
{
    public void Configure(EntityTypeBuilder<RankingOverwrite> builder)
    {
        builder.ToTable("RankingOverwrites", TurnierplanContext.Schema);

        builder.HasKey("TournamentId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PlacementRank)
            .IsRequired();

        builder.Property(x => x.HideRanking)
            .IsRequired();

        builder.HasOne(x => x.AssignTeam)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired();
    }
}

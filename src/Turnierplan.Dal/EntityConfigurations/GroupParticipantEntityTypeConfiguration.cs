using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class GroupParticipantEntityTypeConfiguration : IEntityTypeConfiguration<GroupParticipant>
{
    public void Configure(EntityTypeBuilder<GroupParticipant> builder)
    {
        builder.ToTable("GroupParticipants", TurnierplanContext.Schema);

        builder.HasKey("TournamentId", "GroupId", "TeamId");

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Ignore(x => x.Statistics);
    }
}

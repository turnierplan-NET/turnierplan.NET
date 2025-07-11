using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Planning;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class InvitationLinkEntryEntityTypeConfiguration : IEntityTypeConfiguration<InvitationLinkEntry>
{
    public void Configure(EntityTypeBuilder<InvitationLinkEntry> builder)
    {
        builder.ToTable("PL_InvitationLinkEntries", TurnierplanContext.Schema);

        builder.HasKey("PlanningRealmId", "InvitationLinkId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasOne<TournamentClass>(x => x.Class)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.MaxTeamsPerRegistration);

        builder.Property(x => x.AllowNewRegistrations)
            .IsRequired();
    }
}

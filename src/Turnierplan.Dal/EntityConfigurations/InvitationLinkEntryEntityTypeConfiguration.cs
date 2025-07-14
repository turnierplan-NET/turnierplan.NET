using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class InvitationLinkEntryEntityTypeConfiguration : IEntityTypeConfiguration<InvitationLinkEntry>
{
    public void Configure(EntityTypeBuilder<InvitationLinkEntry> builder)
    {
        builder.ToTable("InvitationLinkEntries", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

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

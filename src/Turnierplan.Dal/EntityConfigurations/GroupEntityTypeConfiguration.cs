using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Tournament;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups", TurnierplanContext.Schema);

        builder.HasKey("TournamentId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.AlphabeticalId);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(ValidationConstants.Group.MaxDisplayNameLength);

        builder.HasMany<Match>()
            .WithOne(x => x.Group)
            .HasForeignKey("TournamentId", "GroupId")
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(x => x.Participants)
            .WithOne(x => x.Group)
            .HasForeignKey("TournamentId", "GroupId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(Group.Participants))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

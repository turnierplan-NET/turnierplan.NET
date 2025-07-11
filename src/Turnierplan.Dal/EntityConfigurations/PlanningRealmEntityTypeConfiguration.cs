using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Planning;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class RealmEntityTypeConfiguration : IEntityTypeConfiguration<PlanningRealm>
{
    public void Configure(EntityTypeBuilder<PlanningRealm> builder)
    {
        builder.ToTable("PL_Realms", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.HasMany(x => x.RoleAssignments)
            .WithOne(x => x.Scope)
            .HasForeignKey("RealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.PlanningRealm.MaxNameLength);

        builder.HasMany(x => x.TournamentClasses)
            .WithOne()
            .HasForeignKey("RealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.InvitationLinks)
            .WithOne()
            .HasForeignKey("RealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(PlanningRealm.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(PlanningRealm.TournamentClasses))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(PlanningRealm.InvitationLinks))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

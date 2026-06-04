using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TournamentPlannerEntityTypeConfiguration : IEntityTypeConfiguration<TournamentPlanner>
{
    public void Configure(EntityTypeBuilder<TournamentPlanner> builder)
    {
        // TODO: Update table name
        builder.ToTable("PlanningRealms", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.HasMany(x => x.RoleAssignments)
            .WithOne(x => x.Scope)
            .HasForeignKey("TournamentPlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(x => x.TournamentClasses)
            .WithOne()
            .HasForeignKey("TournamentPlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.InvitationLinks)
            .WithOne(x => x.TournamentPlanner)
            .HasForeignKey("TournamentPlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Applications)
            .WithOne(x => x.TournamentPlanner)
            .HasForeignKey("TournamentPlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Labels)
            .WithOne()
            .HasForeignKey("TournamentPlannerId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(TournamentPlanner.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.TournamentClasses))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.InvitationLinks))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.Applications))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.Labels))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.TournamentPlanner;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TournamentPlannerEntityTypeConfiguration : IEntityTypeConfiguration<TournamentPlanner>
{
    public void Configure(EntityTypeBuilder<TournamentPlanner> builder)
    {
        // TODO: Update table name and foreign key naming to 'tournament planner' when database schema naming is overhauled.

        // For 2026.3, the naming of 'planning realm' was changed to 'tournament planner' in all application code occurrences.
        // However, the database table and index names were not changed. This was done to prevent bloated and/or redundant
        // migrations in the case that at some point in the future, the database schema naming will be overhauled.

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
            .HasForeignKey("PlanningRealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.HasMany(x => x.TournamentClasses)
            .WithOne()
            .HasForeignKey("PlanningRealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.InvitationLinks)
            .WithOne(x => x.TournamentPlanner)
            .HasForeignKey("PlanningRealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Applications)
            .WithOne(x => x.TournamentPlanner)
            .HasForeignKey("PlanningRealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Labels)
            .WithOne()
            .HasForeignKey("PlanningRealmId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(TournamentPlanner.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.TournamentClasses))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.InvitationLinks))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.Applications))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(TournamentPlanner.Labels))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

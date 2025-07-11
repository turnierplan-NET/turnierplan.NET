using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Planning;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class RealmEntityTypeConfiguration : IEntityTypeConfiguration<Realm>
{
    public void Configure(EntityTypeBuilder<Realm> builder)
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
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.Realm.MaxNameLength);

        builder.HasMany(x => x.Classes)
            .WithOne()
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(Realm.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Realm.Classes))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

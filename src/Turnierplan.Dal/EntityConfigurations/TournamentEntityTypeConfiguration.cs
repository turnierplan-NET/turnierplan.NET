using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Image;
using Turnierplan.Core.Tournament;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class TournamentEntityTypeConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.ToTable("Tournaments", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.IsMigrated)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.Tournament.MaxNameLength);

        builder.Property(x => x.Visibility)
            .IsRequired();

        builder.Ignore(x => x.IsPublic);

        builder.Property(x => x.PublicPageViews)
            .IsRequired();

        builder.OwnsOne(x => x.MatchPlanConfiguration, x =>
        {
            x.ToJson();

            x.OwnsOne(y => y.GroupRoundConfig);
            x.OwnsOne(y => y.FinalsRoundConfig, y =>
            {
                y.OwnsMany(z => z.AdditionalPlayoffs);
            });
            x.OwnsOne(y => y.ScheduleConfig);
        });

        builder.OwnsOne(x => x.ComputationConfiguration, x =>
        {
            x.ToJson();
        });

        builder.OwnsOne(x => x.PresentationConfiguration, x =>
        {
            x.ToJson();

            x.OwnsOne(y => y.Header1);
            x.OwnsOne(y => y.Header2);
        });

        builder.HasOne<Image>(x => x.OrganizerLogo)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne<Image>(x => x.SponsorLogo)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne<Image>(x => x.SponsorBanner)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Ignore(x => x.StartTimestamp);

        builder.Ignore(x => x.EndTimestamp);

        builder.HasMany(x => x.Teams)
            .WithOne()
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Groups)
            .WithOne()
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Matches)
            .WithOne()
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(x => x.Documents)
            .WithOne(x => x.Tournament)
            .HasForeignKey("TournamentId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(Tournament.Teams))!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Tournament.Groups))!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Tournament.Matches))!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindNavigation(nameof(Tournament.Documents))!.SetPropertyAccessMode(PropertyAccessMode.Field);

    }
}

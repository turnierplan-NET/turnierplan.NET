using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Image;
using Turnierplan.Core.Planning;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class InvitationLinkEntityTypeConfiguration : IEntityTypeConfiguration<InvitationLink>
{
    public void Configure(EntityTypeBuilder<InvitationLink> builder)
    {
        builder.ToTable("InvitationLinks", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.Title)
            .HasMaxLength(ValidationConstants.InvitationLink.MaxTitleLength);

        builder.Property(x => x.Description)
            .HasMaxLength(ValidationConstants.InvitationLink.MaxDescriptionLength);

        builder.Property(x => x.ColorCode)
            .IsRequired()
            .HasMaxLength(ValidationConstants.InvitationLink.MaxColorCodeLength);

        builder.Property(x => x.ValidUntil);

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(ValidationConstants.InvitationLink.MaxContactPersonLength);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(ValidationConstants.InvitationLink.MaxContactEmailLength);

        builder.Property(x => x.ContactTelephone)
            .HasMaxLength(ValidationConstants.InvitationLink.MaxContactTelephoneLength);

        builder.HasOne<Image>(x => x.PrimaryLogo)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne<Image>(x => x.SecondaryLogo)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.OwnsMany(x => x.ExternalLinks, x =>
        {
            x.ToJson();
        });

        builder.HasMany(x => x.Entries)
            .WithOne()
            .HasForeignKey("PlanningRealmId", "InvitationLinkId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(InvitationLink.Entries))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

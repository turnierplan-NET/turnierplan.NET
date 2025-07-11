using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Planning;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ApplicationEntityTypeConfiguration : IEntityTypeConfiguration<Application>
{
    public void Configure(EntityTypeBuilder<Application> builder)
    {
        builder.ToTable("Applications", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasOne<InvitationLink>(x => x.SourceLink)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(x => x.Tag)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(ValidationConstants.Application.MaxNameLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(ValidationConstants.Application.MaxEmailLength)
            .IsRequired(false);

        builder.Property(x => x.Telephone)
            .HasMaxLength(ValidationConstants.Application.MaxTelephoneLength)
            .IsRequired(false);

        builder.Property(x => x.CommentFromSender)
            .HasMaxLength(ValidationConstants.Application.MaxCommentFromSenderLength)
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasMaxLength(ValidationConstants.Application.MaxNotesLength)
            .IsRequired();

        builder.HasMany(x => x.Teams)
            .WithOne()
            .HasForeignKey("ApplicationId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

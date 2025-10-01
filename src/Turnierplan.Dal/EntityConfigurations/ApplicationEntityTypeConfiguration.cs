using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.PlanningRealm;

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

        builder.Property(x => x.Notes)
            .IsRequired();

        builder.Property(x => x.NotesHistory)
            .IsRequired();

        builder.Property(x => x.Contact)
            .IsRequired();

        builder.Property(x => x.ContactEmail)
            .IsRequired(false);

        builder.Property(x => x.ContactTelephone)
            .IsRequired(false);

        builder.Property(x => x.Comment)
            .IsRequired(false);

        builder.Property(x => x.FormSession)
            .IsRequired(false);

        builder.HasIndex(x => x.FormSession)
            .IsUnique();

        builder.HasMany(x => x.Teams)
            .WithOne(x => x.Application)
            .HasForeignKey("ApplicationId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}

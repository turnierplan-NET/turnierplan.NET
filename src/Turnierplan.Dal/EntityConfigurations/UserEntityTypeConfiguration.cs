using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.User;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.User.MaxNameLength);

        builder.Property(x => x.EMail)
            .IsRequired()
            .HasMaxLength(ValidationConstants.User.MaxEMailLength);

        builder.Property(x => x.NormalizedEMail)
            .IsRequired()
            .HasMaxLength(ValidationConstants.User.MaxEMailLength);

        builder.HasIndex(x => x.NormalizedEMail)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.LastPasswordChange)
            .IsRequired();

        builder.Property(x => x.SecurityStamp)
            .IsRequired();

        builder.HasMany(x => x.Organizations)
            .WithOne()
            .HasForeignKey("OwnerId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}

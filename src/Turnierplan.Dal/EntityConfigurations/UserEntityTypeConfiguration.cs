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

        builder.Property(x => x.PrincipalId)
            .IsRequired();

        builder.HasIndex(x => x.PrincipalId)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.EMail)
            .IsRequired();

        builder.Property(x => x.NormalizedEMail)
            .IsRequired();

        builder.HasIndex(x => x.NormalizedEMail)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.LastPasswordChange)
            .IsRequired();

        builder.Property(x => x.SecurityStamp)
            .IsRequired();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Planning;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ClassEntityTypeConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("PL_Classes", TurnierplanContext.Schema);

        builder.HasKey("RealmId", "Id");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.Class.MaxNameLength);

        builder.Property(x => x.MaxTeamCount);
    }
}

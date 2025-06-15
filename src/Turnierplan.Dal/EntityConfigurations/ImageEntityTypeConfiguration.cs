using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.Image;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class ImageEntityTypeConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.HasMany(x => x.RoleAssignments)
            .WithOne(x => x.Scope)
            .HasForeignKey("ImageId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.ResourceIdentifier)
            .IsRequired();

        builder.HasIndex(x => x.ResourceIdentifier)
            .IsUnique();

        builder.Property(x => x.PublicId)
            .HasConversion<PublicIdConverter>();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.Image.MaxNameLength);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(ValidationConstants.Image.MaxFileTypeLength);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.Width)
            .IsRequired();

        builder.Property(x => x.Height)
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(Image.RoleAssignments))!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

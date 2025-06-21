using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.EntityConfigurations;

public sealed class RoleAssignmentEntityTypeConfiguration<T> : IEntityTypeConfiguration<RoleAssignment<T>>
    where T : Entity, IEntityWithRoleAssignments<T>
{
    public void Configure(EntityTypeBuilder<RoleAssignment<T>> builder)
    {
        builder.ToTable($"IAM_{typeof(T).Name}", TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.Principal)
            .IsRequired()
            .HasConversion<PrincipalConverter>();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(ValidationConstants.RoleAssignment.MaxDescriptionLength);
    }
}

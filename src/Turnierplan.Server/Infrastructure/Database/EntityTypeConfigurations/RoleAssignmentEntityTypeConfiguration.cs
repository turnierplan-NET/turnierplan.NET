using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.RoleAssignments;

namespace Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

internal sealed class RoleAssignmentEntityTypeConfiguration<T> : IEntityTypeConfiguration<RoleAssignment<T>>
    where T : EntityBase<long>
{
    private readonly string _entityName;

    public RoleAssignmentEntityTypeConfiguration(string? entityName = null)
    {
        _entityName = entityName ?? typeof(T).Name.ToLower();
    }

    public void Configure(EntityTypeBuilder<RoleAssignment<T>> builder)
    {
        var tableName = $"{_entityName}_role_assignment";
        builder.ToTable(tableName, TurnierplanContext.Schema);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Scope)
            .WithMany()
            .HasForeignKey($"{_entityName}_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.PrincipalId)
            .IsRequired();

        builder.HasIndex(x => x.PrincipalId);

        builder.Property(x => x.PrincipalKind)
            .IsRequired();

        builder.HasIndex(x => x.PrincipalKind);
    }
}

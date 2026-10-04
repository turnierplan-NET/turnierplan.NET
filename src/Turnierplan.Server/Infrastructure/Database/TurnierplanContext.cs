using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Infrastructure.Database.EntityTypeConfigurations;

namespace Turnierplan.Server.Infrastructure.Database;

internal sealed class TurnierplanContext : DbContext
{
    public const string Schema = "turnierplan_v2"; // TODO: Determine new schema name

    private readonly ILogger<TurnierplanContext> _logger;

    public TurnierplanContext(DbContextOptions<TurnierplanContext> options, ILogger<TurnierplanContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<RoleAssignment<Organization>> OrganizationRoleAssignments { get; set; } = null!;

    public DbSet<RoleAssignment<T>> GetRoleAssignmentsForType<T>()
        where T : EntityBase<long>
    {
        var contextType = typeof(TurnierplanContext);
        var prop = contextType.GetProperty($"{typeof(T).Name}RoleAssignments");

        return prop?.GetValue(this) as DbSet<RoleAssignment<T>>
            ?? throw new InvalidOperationException($"Could not find or access role assignments set for type '{typeof(T).FullName}'.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrganizationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Organization>());
    }
}

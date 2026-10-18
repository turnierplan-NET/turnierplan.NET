using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.ApiKeys;
using Turnierplan.Server.Domain.Organizations;
using Turnierplan.Server.Domain.Projects;
using Turnierplan.Server.Domain.RoleAssignments;
using Turnierplan.Server.Domain.Users;
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

    public DbSet<ApiKey> ApiKeys { get; set; } = null!;

    public DbSet<RoleAssignment<ApiKey>> ApiKeyRoleAssignments { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<RoleAssignment<Project>> ProjectRoleAssignments { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<RoleAssignment<Organization>> OrganizationRoleAssignments { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApiKeyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyRequestEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<ApiKey>("api_key"));
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Project>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Organization>());
    }
}

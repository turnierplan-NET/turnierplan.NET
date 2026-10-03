using Microsoft.EntityFrameworkCore;
using Turnierplan.Server.Domain.Organizations;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrganizationEntityTypeConfiguration());
        // TODO: Add EF naming conventions for postgres
        // TODO: Entity configs
    }
}

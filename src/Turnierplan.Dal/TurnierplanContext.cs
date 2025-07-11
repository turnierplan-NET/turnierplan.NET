using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Document;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.Planning;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Turnierplan.Core.Venue;
using Turnierplan.Dal.EntityConfigurations;

namespace Turnierplan.Dal;

public sealed class TurnierplanContext : DbContext, IUnitOfWork
{
    public const string Schema = "turnierplan";

    private readonly ILogger<TurnierplanContext> _logger;

    private IDbContextTransaction? _activeTransaction;

    public TurnierplanContext(DbContextOptions<TurnierplanContext> options, ILogger<TurnierplanContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<ApiKey> ApiKeys { get; set; } = null!;

    public DbSet<RoleAssignment<ApiKey>> ApiKeyRoleAssignments { get; set; } = null!;

    public DbSet<ApiKeyRequest> ApiKeyRequests { get; set; } = null!;

    public DbSet<Document> Documents { get; set; } = null!;

    public DbSet<Folder> Folders { get; set; } = null!;

    public DbSet<RoleAssignment<Folder>> FolderRoleAssignments { get; set; } = null!;

    public DbSet<Group> Groups { get; set; } = null!;

    public DbSet<GroupParticipant> GroupParticipants { get; set; } = null!;

    public DbSet<Image> Images { get; set; } = null!;

    public DbSet<RoleAssignment<Image>> ImageRoleAssignments { get; set; } = null!;

    public DbSet<Match> Matches { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<RoleAssignment<Organization>> OrganizationRoleAssignments { get; set; } = null!;

    public DbSet<Realm> Realms { get; set; } = null!;

    public DbSet<RoleAssignment<Realm>> RealmRoleAssignments { get; set; } = null!;

    public DbSet<Team> Teams { get; set; } = null!;

    public DbSet<Tournament> Tournaments { get; set; } = null!;

    public DbSet<RoleAssignment<Tournament>> TournamentRoleAssignments { get; set; } = null!;

    public DbSet<TournamentClass> TournamentClasses { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Venue> Venues { get; set; } = null!;

    public DbSet<RoleAssignment<Venue>> VenueRoleAssignments { get; set; } = null!;

    public async Task BeginTransactionAsync()
    {
        if (_activeTransaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _logger.LogInformation("Beginning database transaction");

        _activeTransaction = await Database.BeginTransactionAsync().ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync()
    {
        if (_activeTransaction is null)
        {
            throw new InvalidOperationException("Transaction not started.");
        }

        _logger.LogInformation("Committing database transaction");

        await _activeTransaction.CommitAsync().ConfigureAwait(false);
        await _activeTransaction.DisposeAsync().ConfigureAwait(false);

        _activeTransaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_activeTransaction is null)
        {
            throw new InvalidOperationException("Transaction not started.");
        }

        _logger.LogInformation("Rolling back database transaction");

        await _activeTransaction.RollbackAsync().ConfigureAwait(false);
        await _activeTransaction.DisposeAsync().ConfigureAwait(false);

        _activeTransaction = null;
    }

    public override void Dispose()
    {
        _activeTransaction?.Dispose();
        _activeTransaction = null;

        base.Dispose();
    }

    public override async ValueTask DisposeAsync()
    {
        if (_activeTransaction is not null)
        {
            await _activeTransaction.DisposeAsync().ConfigureAwait(false);
            _activeTransaction = null;
        }

        await base.DisposeAsync().ConfigureAwait(false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApiKeyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ApiKeyRequestEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new FolderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GroupParticipantEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ImageEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MatchEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RealmEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TeamEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TournamentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TournamentClassEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VenueEntityTypeConfiguration());

        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<ApiKey>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Folder>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Image>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Organization>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Realm>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Tournament>());
        modelBuilder.ApplyConfiguration(new RoleAssignmentEntityTypeConfiguration<Venue>());
    }
}

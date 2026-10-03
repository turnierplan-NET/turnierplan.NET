using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Turnierplan.Server.Infrastructure.Database.Extensions;

internal static class ServiceCollectionExtensions
{
    public static void AddTurnierplanContext(this IServiceCollection services, IConfiguration configuration)
    {
        string? inMemoryDatabaseName = null;

        services.AddDbContext<TurnierplanContext>((sp, options) =>
        {
            if (sp.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            if (configuration.UseInMemoryDatabase())
            {
                inMemoryDatabaseName ??= Guid.NewGuid().ToString();
                options.UseInMemoryDatabase(inMemoryDatabaseName);
                options.ConfigureWarnings(x => x.Log(InMemoryEventId.TransactionIgnoredWarning));
            }
            else
            {
                var connectionString = configuration.GetDatabaseConnectionString();

                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(TurnierplanContext).Assembly.GetName().Name);
                    npgsqlOptions.MigrationsHistoryTable("__EFMigrationHistory", TurnierplanContext.Schema);
                });

                options.UseSnakeCaseNamingConvention();
            }
        });

        // TODO: Add repositories, if required

        /*
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationChangeLogRepository, ApplicationChangeLogRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<IInvitationLinkRepository, InvitationLinkRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
#if TP_RESOURCE_PLANNER
        services.AddScoped<IResourcePlannerRepository, ResourcePlannerRepository>();
#endif
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<ITournamentPlannerRepository, TournamentPlannerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();

        services.AddScoped<IRepositoryWithPublicId<ApiKey, long>>(sp => sp.GetRequiredService<IApiKeyRepository>());
        services.AddScoped<IRepositoryWithPublicId<Document, long>>(sp => sp.GetRequiredService<IDocumentRepository>());
        services.AddScoped<IRepositoryWithPublicId<Folder, long>>(sp => sp.GetRequiredService<IFolderRepository>());
        services.AddScoped<IRepositoryWithPublicId<InvitationLink, long>>(sp => sp.GetRequiredService<IInvitationLinkRepository>());
        services.AddScoped<IRepositoryWithPublicId<Image, long>>(sp => sp.GetRequiredService<IImageRepository>());
        services.AddScoped<IRepositoryWithPublicId<Organization, long>>(sp => sp.GetRequiredService<IOrganizationRepository>());
#if TP_RESOURCE_PLANNER
        services.AddScoped<IRepositoryWithPublicId<ResourcePlanner, long>>(sp => sp.GetRequiredService<IResourcePlannerRepository>());
#endif
        services.AddScoped<IRepositoryWithPublicId<Tournament, long>>(sp => sp.GetRequiredService<ITournamentRepository>());
        services.AddScoped<IRepositoryWithPublicId<TournamentPlanner, long>>(sp => sp.GetRequiredService<ITournamentPlannerRepository>());
        services.AddScoped<IRepositoryWithPublicId<Venue, long>>(sp => sp.GetRequiredService<IVenueRepository>());

        services.AddScoped<IRoleAssignmentRepository<ApiKey>, ApiKeyRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Folder>, FolderRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Image>, ImageRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Organization>, OrganizationRoleAssignmentRepository>();
#if TP_RESOURCE_PLANNER
        services.AddScoped<IRoleAssignmentRepository<ResourcePlanner>, ResourcePlannerRoleAssignmentRepository>();
#endif
        services.AddScoped<IRoleAssignmentRepository<Tournament>, TournamentRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<TournamentPlanner>, TournamentPlannerRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Venue>, VenueRoleAssignmentRepository>();

        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<ApiKey>>());
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<Folder>>());
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<Image>>());
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<Organization>>());
#if TP_RESOURCE_PLANNER
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<ResourcePlanner>>());
#endif
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<Tournament>>());
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<TournamentPlanner>>());
        services.AddScoped<IRoleAssignmentRepository>(sp => sp.GetRequiredService<IRoleAssignmentRepository<Venue>>());
        */
    }
}

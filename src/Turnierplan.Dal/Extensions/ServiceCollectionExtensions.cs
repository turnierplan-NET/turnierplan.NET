using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Document;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTurnierplanDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
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
            }
        });

        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationChangeLogRepository, ApplicationChangeLogRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<IInvitationLinkRepository, InvitationLinkRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IPlanningRealmRepository, PlanningRealmRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();

        services.AddScoped<IRepositoryWithPublicId<ApiKey, long>>(sp => sp.GetRequiredService<IApiKeyRepository>());
        services.AddScoped<IRepositoryWithPublicId<Document, long>>(sp => sp.GetRequiredService<IDocumentRepository>());
        services.AddScoped<IRepositoryWithPublicId<Folder, long>>(sp => sp.GetRequiredService<IFolderRepository>());
        services.AddScoped<IRepositoryWithPublicId<InvitationLink, long>>(sp => sp.GetRequiredService<IInvitationLinkRepository>());
        services.AddScoped<IRepositoryWithPublicId<Image, long>>(sp => sp.GetRequiredService<IImageRepository>());
        services.AddScoped<IRepositoryWithPublicId<Organization, long>>(sp => sp.GetRequiredService<IOrganizationRepository>());
        services.AddScoped<IRepositoryWithPublicId<PlanningRealm, long>>(sp => sp.GetRequiredService<IPlanningRealmRepository>());
        services.AddScoped<IRepositoryWithPublicId<Tournament, long>>(sp => sp.GetRequiredService<ITournamentRepository>());
        services.AddScoped<IRepositoryWithPublicId<Venue, long>>(sp => sp.GetRequiredService<IVenueRepository>());

        services.AddScoped<IRoleAssignmentRepository<ApiKey>, ApiKeyRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Folder>, FolderRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Image>, ImageRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Organization>, OrganizationRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<PlanningRealm>, PlanningRealmRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Tournament>, TournamentRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Venue>, VenueRoleAssignmentRepository>();
    }
}

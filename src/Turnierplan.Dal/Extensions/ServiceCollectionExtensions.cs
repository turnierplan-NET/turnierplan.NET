using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Document;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Turnierplan.Core.Venue;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTurnierplanDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TurnierplanContext>((sp, options) =>
        {
            if (sp.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }

            var connectionString = configuration.GetDatabaseConnectionString();

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(TurnierplanContext).Assembly.GetName().Name);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationHistory", TurnierplanContext.Schema);
            });
        });

        var applicationInsightsConnectionString = configuration.GetSection("ApplicationInsights").GetValue<string>("ConnectionString");

        if (!string.IsNullOrEmpty(applicationInsightsConnectionString))
        {
            services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddNpgsql()
                        .AddAzureMonitorTraceExporter(o => o.ConnectionString = applicationInsightsConnectionString);
                });
        }

        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();

        services.AddScoped<IRoleAssignmentRepository<ApiKey>, ApiKeyRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Folder>, FolderRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Image>, ImageRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Organization>, OrganizationRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Tournament>, TournamentRoleAssignmentRepository>();
        services.AddScoped<IRoleAssignmentRepository<Venue>, VenueRoleAssignmentRepository>();

        return services;
    }
}

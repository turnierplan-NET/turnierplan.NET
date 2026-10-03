using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Turnierplan.Server.Infrastructure.Authorization.Extensions;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints.Extensions;
using Turnierplan.Server.Infrastructure.Json;
using Turnierplan.Server.Infrastructure.Middlewares;
using Turnierplan.Server.Options;

namespace Turnierplan.Server;

internal sealed class TurnierplanServer
{
    private readonly WebApplication _application;

    static TurnierplanServer()
    {
        // TODO: Enable once FluentValidation is installed
        //ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    public TurnierplanServer(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<TurnierplanOptions>(builder.Configuration.GetSection("Turnierplan"));

        // builder.Services.AddTurnierplanMonitoring(builder.Configuration);
        builder.Services.AddTurnierplanContext(builder.Configuration);
        // builder.Services.AddTurnierplanDocumentRendering<ApplicationUrlProvider>();
        // builder.Services.AddTurnierplanImageStorage<DatabaseImageProvider>(builder.Configuration.GetSection("ImageStorage"));
        // builder.Services.AddTurnierplanLocalization();
        // builder.Services.AddTurnierplanSecurity(builder.Configuration.GetSection("Identity"));
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorizationBuilder().AddTurnierplanPolicies();

        builder.Services.AddValidatorsFromAssembly(typeof(TurnierplanServer).Assembly, includeInternalTypes: true);
        // builder.Services.AddSingleton<IMapper, Mapper>();
        // builder.Services.AddScoped<IDeletionHelper, DeletionHelper>();

        builder.Services.AddHealthChecks();
        // builder.Services.AddRazorPages();

        // builder.Services.AddOpenApi("turnierplan", options =>
        // {
        //     options.AddOperationTransformer<ResponseOperationTransformer>();
        //     options.AddSchemaTransformer<EnumSchemaTransformer>();
        //     options.AddSchemaTransformer<NumberSchemaTransformer>();
        // });

        builder.Services.Configure<JsonOptions>(configure =>
        {
            configure.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            configure.SerializerOptions.PropertyNameCaseInsensitive = true;
            configure.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            configure.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            configure.SerializerOptions.Converters.Add(new JsonPublicIdConverter());
        });

        builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

        _application = builder.Build();

//         if (app.Environment.IsDevelopment())
//         {
//             app.MapOpenApi();
//
//             app.MapScalarApiReference(configure =>
//             {
//                 configure.Title = "turnierplan.NET";
//                 configure.Favicon = "/favicon.ico";
//                 configure.OpenApiRoutePattern = "/openapi/turnierplan.json";
//             });
//         }
//
//         app.MapHealthChecks("/health");
//
         _application.UseAuthentication();
         _application.UseAuthorization();

         _application.UseMiddleware<VersionMiddleware>();
//
//         app.Map(string.Empty, (HttpContext context) =>
//         {
//             context.Response.StatusCode = StatusCodes.Status303SeeOther;
//             context.Response.Headers.Location = "/portal/login";
//         });
//
        _application.MapTurnierplanEndpoints();
//         app.MapImageStorageEndpoint();
//         app.MapRazorPages();
//
//         app.Use((httpContext, next) =>
//         {
//             if (httpContext.Request.Path.HasValue && (httpContext.Request.Path.Value.Equals("/portal") || httpContext.Request.Path.Value.StartsWith("/portal/")))
//             {
//                 httpContext.Request.Path = "/portal.html";
//                 httpContext.Response.Headers.Append("Cache-Control", "no-store");
//
//                 // Set endpoint to null so the static files middleware will handle the request.
//                 httpContext.SetEndpoint(null);
//             }
//
//             return next(httpContext);
//         });
//
//         app.UseDefaultFiles();
//         app.UseStaticFiles();
//
// // Migrate database and create admin user if DB is empty
//         await app.InitializeDatabaseAsync();
//
// // Run image storage migrations if required by the configured image storage
//         await app.MigrateImageStorageAsync();
    }

    public async Task RunAsync()
    {
        await _application.RunAsync();
    }
}

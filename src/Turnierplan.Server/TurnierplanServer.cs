using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using Turnierplan.Server.Infrastructure.Authentication.Extensions;
using Turnierplan.Server.Infrastructure.Authorization.Extensions;
using Turnierplan.Server.Infrastructure.Database.Extensions;
using Turnierplan.Server.Infrastructure.Endpoints;
using Turnierplan.Server.Infrastructure.Endpoints.Extensions;
using Turnierplan.Server.Infrastructure.Json;
using Turnierplan.Server.Infrastructure.Middlewares;
using Turnierplan.Server.Infrastructure.Monitoring.Extensions;
using Turnierplan.Server.Infrastructure.OpenApi;
using Turnierplan.Server.Options;

namespace Turnierplan.Server;

internal sealed class TurnierplanServer
{
    private readonly WebApplication _application;

    static TurnierplanServer()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    public TurnierplanServer(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<TurnierplanOptions>(builder.Configuration.GetSection("Turnierplan"));

        builder.Services.AddTurnierplanMonitoring(builder.Configuration);
        builder.Services.AddTurnierplanContext(builder.Configuration);
        // builder.Services.AddTurnierplanDocumentRendering<ApplicationUrlProvider>();
        // builder.Services.AddTurnierplanImageStorage<DatabaseImageProvider>(builder.Configuration.GetSection("ImageStorage"));
        // builder.Services.AddTurnierplanLocalization();
        builder.Services.AddTurnierplanAuthentication();
        builder.Services.AddTurnierplanAuthorization();

        builder.Services.AddValidatorsFromAssembly(typeof(TurnierplanServer).Assembly, lifetime: ServiceLifetime.Singleton, includeInternalTypes: true);

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            // TODO: Make configurable via options
            options.IdleTimeout = TimeSpan.FromHours(12);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddHealthChecks();
        // builder.Services.AddRazorPages();

        builder.Services.AddOpenApi("turnierplan", options =>
        {
            options.AddOperationTransformer<ResponseOperationTransformer>();
            options.AddSchemaTransformer<EnumSchemaTransformer>();
            options.AddSchemaTransformer<NumberSchemaTransformer>();
        });

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

         if (_application.Environment.IsDevelopment())
         {
             _application.MapOpenApi();

             _application.MapScalarApiReference(configure =>
             {
                 configure.Title = "turnierplan.NET";
                 configure.Favicon = "/favicon.ico";
                 configure.OpenApiRoutePattern = "/openapi/turnierplan.json";
             });
         }

         _application.MapHealthChecks("/health");

         _application.UseSession();
         _application.UseAuthentication();
         _application.UseAuthorization();

         _application.UseMiddleware<VersionMiddleware>();

//         app.Map(string.Empty, (HttpContext context) =>
//         {
//             context.Response.StatusCode = StatusCodes.Status303SeeOther;
//             context.Response.Headers.Location = "/portal/login";
//         });

        _application.MapAuthenticationEndpoints();
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

        _application.UseDefaultFiles();
        _application.UseStaticFiles();

        _application.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

                switch (exception)
                {
                    case ValidationException validationException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(validationException.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessage));
                        break;
                    case PermissionException:
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = MediaTypeNames.Text.Plain;
                        await context.Response.WriteAsync("The authenticated user does not have the necessary permissions to access/modify the target resource.");
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = MediaTypeNames.Text.Plain;
                        await context.Response.WriteAsync("An unexpected error occurred while processing the request.");
                        break;
                }
            }
        });
    }

    public async Task RunAsync()
    {
        // Migrate database and create admin user if DB is empty
        await _application.InitializeDatabaseAsync();
//
// // Run image storage migrations if required by the configured image storage
//         await app.MigrateImageStorageAsync();

        await _application.RunAsync();
    }
}

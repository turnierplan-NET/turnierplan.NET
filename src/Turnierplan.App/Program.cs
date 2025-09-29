using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;
using Turnierplan.App.Constants;
using Turnierplan.App.Converters;
using Turnierplan.App.Extensions;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.OpenApi;
using Turnierplan.App.Options;
using Turnierplan.Dal.Extensions;
using Turnierplan.ImageStorage.Extensions;
using Turnierplan.Localization.Extensions;
using Turnierplan.PdfRendering.Extensions;

Console.WriteLine();
Console.WriteLine(  "  __                                                     ___                                        __");
Console.WriteLine( @" /\ \__                        __                       /\_ \                                      /\ \__");
Console.WriteLine( @" \ \ ,_\  __  __  _ __    ___ /\_\     __   _ __   _____\//\ \      __      ___         ___      __\ \ ,_\");
Console.WriteLine( @"  \ \ \/ /\ \/\ \/\`'__\/' _ `\/\ \  /'__`\/\`'__\/\ '__`\\ \ \   /'__`\  /' _ `\     /' _ `\  /'__`\ \ \/");
Console.WriteLine( @"   \ \ \_\ \ \_\ \ \ \/ /\ \/\ \ \ \/\  __/\ \ \/ \ \ \L\ \\_\ \_/\ \L\.\_/\ \/\ \  __/\ \/\ \/\  __/\ \ \_");
Console.WriteLine( @"    \ \__\\ \____/\ \_\ \ \_\ \_\ \_\ \____\\ \_\  \ \ ,__//\____\ \__/.\_\ \_\ \_\/\_\ \_\ \_\ \____\\ \__\");
Console.WriteLine( @"     \/__/ \/___/  \/_/  \/_/\/_/\/_/\/____/ \/_/   \ \ \/ \/____/\/__/\/_/\/_/\/_/\/_/\/_/\/_/\/____/ \/__/");
Console.WriteLine( @"                                                     \ \_\");
Console.WriteLine($@"                                                      \/_/   v{TurnierplanVersion.Version}");
Console.WriteLine();

ValidatorOptions.Global.LanguageManager.Enabled = false;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<TurnierplanOptions>(builder.Configuration.GetSection("Turnierplan"));

builder.Services.AddTurnierplanDataAccessLayer(builder.Configuration);
builder.Services.AddTurnierplanDocumentRendering<ApplicationUrlProvider>();
builder.Services.AddTurnierplanImageStorage(builder.Configuration.GetSection("ImageStorage"));
builder.Services.AddTurnierplanLocalization();
builder.Services.AddTurnierplanSecurity(builder.Configuration.GetSection("Identity"));

builder.Services.AddSingleton<IMapper, Mapper>();
builder.Services.AddScoped<IDeletionHelper, DeletionHelper>();

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();

builder.Services.AddOpenApi("turnierplan", options =>
{
    options.AddOperationTransformer<PdfResponseOperationTransformer>();
    options.AddSchemaTransformer<EnumSchemaTransformer>();
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(configure =>
    {
        configure.Title = "turnierplan.NET";
        configure.Favicon = "/favicon.ico";
        configure.OpenApiRoutePattern = "/openapi/turnierplan.json";
    });
}

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.Map(string.Empty, (HttpContext context) =>
{
    context.Response.StatusCode = StatusCodes.Status303SeeOther;
    context.Response.Headers.Location = "/portal/login";
});

app.MapTurnierplanEndpoints();
app.MapImageStorageEndpoint();
app.MapRazorPages();

app.Use((httpContext, next) =>
{
    if (httpContext.Request.Path.HasValue && (httpContext.Request.Path.Value.Equals("/portal") || httpContext.Request.Path.Value.StartsWith("/portal/")))
    {
        httpContext.Request.Path = "/portal.html";
        httpContext.Response.Headers.Append("Cache-Control", "no-store");

        // Set endpoint to null so the static files middleware will handle the request.
        httpContext.SetEndpoint(null);
    }

    return next(httpContext);
});

app.UseDefaultFiles();
app.UseStaticFiles();

// Migrate database and create admin user if DB is empty
await app.InitializeDatabaseAsync();

await app.RunAsync();

// Public class definition is required for functional tests
public sealed partial class Program;

// TODO: Wenn Anmeldung erstellt wird, die Anzahl in der Turnierklassen-Übersicht aktualisieren
// TODO: Button, um alle EMail Adressen zu kopieren in der aktuellen Anmeldungs Übersicht
// TODO: Möglichkeit, Anmeldungen zu löschen
// TODO: Möglichkeit, nachträglich Mannscahft zu Anmeldung hinzuzufügen

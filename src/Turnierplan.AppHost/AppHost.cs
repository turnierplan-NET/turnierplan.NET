using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("turnierplan-postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("turnierplan-database");

builder.AddProject<Turnierplan_App>("turnierplan-backend")
    .WaitFor(database)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("Database__ConnectionString", database.Resource.ConnectionStringExpression);

if (builder.Configuration.GetValue("TURNIERPLAN_ASPIRE_RUN_CLIENT", defaultValue: false))
{
    builder.AddJavaScriptApp("turnierplan-client", "../Turnierplan.App/Client")
        .WithRunScript("start")
        .WithHttpEndpoint(45001, isProxied: false)
        .WithHttpHealthCheck("/index.html")
        .WithNpm(install: true, installCommand: "ci");
}
else
{
    // When starting locally without client app, add an external resource with the client app URL
    // and health check so that the client app is still visible and accessible in the dashboard.
    builder.AddExternalService("turnierplan-client", "http://localhost:45001")
        .WithHttpHealthCheck("/index.html");
}

builder.Build().Run();

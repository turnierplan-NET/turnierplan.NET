using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("Turnierplan-Postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("Turnierplan-Database");

builder.AddProject<Turnierplan_App>("Turnierplan-Backend")
    .WaitFor(database)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("Database__ConnectionString", database.Resource.ConnectionStringExpression);

builder.AddJavaScriptApp("Turnierplan-Client", "../Turnierplan.App/Client")
    .WithRunScript("start")
    .WithHttpEndpoint(45001, isProxied: false)
    .WithHttpHealthCheck("/index.html")
    .WithNpm(installCommand: "ci");

builder.Build().Run();

// TODO: Add OpenTelemetry collection

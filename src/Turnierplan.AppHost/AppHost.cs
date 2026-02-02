using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("Postgres-Server")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("Postgres-Database");

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

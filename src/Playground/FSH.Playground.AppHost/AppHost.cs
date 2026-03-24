var builder = DistributedApplication.CreateBuilder(args);

var dbProvider = builder.Configuration["DatabaseProvider"] ?? "postgresql";

IResourceBuilder<IResourceWithConnectionString> dbResource;
string dbProviderName, migrationsAssembly;

if (dbProvider.Equals("mssql", StringComparison.OrdinalIgnoreCase))
{
    var mssql = builder.AddSqlServer("mssql")
        .WithDataVolume("fsh-mssql-data")
        .AddDatabase("fsh");
    dbResource = mssql;
    dbProviderName = "MSSQL";
    migrationsAssembly = "FSH.Playground.Migrations.MSSQL";
}
else
{
    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume("fsh-postgres-data")
        .AddDatabase("fsh");
    dbResource = postgres;
    dbProviderName = "POSTGRESQL";
    migrationsAssembly = "FSH.Playground.Migrations.PostgreSQL";
}

var redis = builder.AddRedis("redis").WithDataVolume("fsh-redis-data");

builder.AddProject<Projects.Playground_Api>("playground-api")
    .WithReference(dbResource)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("OpenTelemetryOptions__Exporter__Otlp__Endpoint", "https://localhost:4317")
    .WithEnvironment("OpenTelemetryOptions__Exporter__Otlp__Protocol", "grpc")
    .WithEnvironment("OpenTelemetryOptions__Exporter__Otlp__Enabled", "true")
    .WithEnvironment("DatabaseOptions__Provider", dbProviderName)
    .WithEnvironment("DatabaseOptions__ConnectionString", dbResource.Resource.ConnectionStringExpression)
    .WithEnvironment("DatabaseOptions__MigrationsAssembly", migrationsAssembly)
    .WaitFor(dbResource)
    .WithReference(redis)
    .WithEnvironment("CachingOptions__Redis", redis.Resource.ConnectionStringExpression)
    .WithEnvironment("CachingOptions__EnableSsl", "true")
    .WaitFor(redis);

builder.AddProject<Projects.Playground_Blazor>("playground-blazor");

await builder.Build().RunAsync();

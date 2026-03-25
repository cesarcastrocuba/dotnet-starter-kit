using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Testcontainers.MsSql;
using Xunit;
using FSH.Modules.Identity.Data;
using FSH.Modules.Multitenancy.Data;
using Microsoft.EntityFrameworkCore;
using FSH.Framework.Shared.Multitenancy;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Persistence;

namespace FSH.Tests.Shared.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer? _dbPostgreSqlContainer;
    private readonly MsSqlContainer? _dbMsSqlContainer;
    private readonly RedisContainer _redisContainer;
    private string _connectionString { get; set; } = default!;
    private string _dbProvider { get; set; } = "mssql";
    public CustomWebApplicationFactory()
    {      
        if (_dbProvider == "mssql")
        {
            _dbMsSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("fsh_secret_123!")
                .Build();
        }
        else 
        {
            _dbPostgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
                .WithDatabase("fsh_test_b")
                .WithUsername("postgres")
                .WithPassword("fsh_secret_123!")
                .Build();
        }

        _redisContainer = new RedisBuilder("redis:7-alpine")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (_dbProvider == "mssql") 
            _connectionString = _dbMsSqlContainer?.GetConnectionString() ?? throw new InvalidOperationException("MSSQL container is not initialized.");
        else
            _connectionString = _dbPostgreSqlContainer?.GetConnectionString() ?? throw new InvalidOperationException("PostgreSQL container is not initialized.");


        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "DatabaseOptions:ConnectionString", _connectionString },
                { "CachingOptions:Redis", _redisContainer.GetConnectionString() },
                { "MultitenancyOptions:RunTenantMigrationsOnStartup", "true" }
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddTransient<FSH.Framework.Jobs.Services.IJobService, TestJobService>();
        });
    }

    public async Task InitializeAsync()
    {
        if (_dbProvider == "mssql" && _dbMsSqlContainer != null)
            await _dbMsSqlContainer.StartAsync();
        else if (_dbPostgreSqlContainer != null)
            await _dbPostgreSqlContainer.StartAsync();

        await _redisContainer.StartAsync();

        // Ensure database schema is created and root tenant is seeded before tests run
        using var scope = Services.CreateScope();
        
        // 1. Migrate Tenant Catalog
        var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        await tenantDbContext.Database.MigrateAsync();

        // 2. Ensure Root Tenant exists and SET CONTEXT IMMEDIATELY
        var rootTenant = await tenantDbContext.TenantInfo.FindAsync(MultitenancyConstants.Root.Id);
        if (rootTenant is null)
        {
            rootTenant = new AppTenantInfo(
                MultitenancyConstants.Root.Id,
                MultitenancyConstants.Root.Name,
                null,
                MultitenancyConstants.Root.EmailAddress,
                issuer: MultitenancyConstants.Root.Issuer);
            rootTenant.SetValidity(DateTimeOffset.UtcNow.AddYears(1));
            await tenantDbContext.TenantInfo.AddAsync(rootTenant);
            await tenantDbContext.SaveChangesAsync();
        }

        // 3. Set Context for the rest of the initialization
        var setter = scope.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
        setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(rootTenant);

        // 4. Migrate Identity Schema
        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await identityDbContext.Database.MigrateAsync();

        // 5. Seed Identity for Root Tenant
        var initializers = scope.ServiceProvider.GetServices<IDbInitializer>();
        foreach (var initializer in initializers)
        {
            await initializer.SeedAsync(default);
        }
    }

    public new async Task DisposeAsync()
    {
        if (_dbProvider == "mssql" && _dbMsSqlContainer != null)
            await _dbMsSqlContainer.DisposeAsync().AsTask();
        else if (_dbPostgreSqlContainer != null)
            await _dbPostgreSqlContainer.DisposeAsync().AsTask();

        await _redisContainer.DisposeAsync().AsTask();
    }
}

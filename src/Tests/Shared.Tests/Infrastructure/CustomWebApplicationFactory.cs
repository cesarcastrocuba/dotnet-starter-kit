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

namespace FSH.Tests.Shared.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private readonly MsSqlContainer _dbMsSqlContainer;
    private readonly RedisContainer _redisContainer;
    private string _connectionString { get; set; } = default!;
    private string _dbProvider { get; set; } = "mssql";
    public CustomWebApplicationFactory()
    {      
        if (_dbProvider == "mssql")
        {
            _dbMsSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("fsh_secret_123!")
                .Build();
        }
        else 
        {
            _dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("fsh_test_b")
                .WithUsername("postgres")
                .WithPassword("fsh_secret_123!")
                .Build();
        }


        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (_dbProvider == "mssql") 
            _connectionString = _dbMsSqlContainer.GetConnectionString();
        else
            _connectionString = _dbContainer.GetConnectionString();


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
        if (_dbProvider == "mssql")
            await _dbMsSqlContainer.StartAsync();
        else
            await _dbContainer.StartAsync();

        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_dbProvider == "mssql")
            await _dbMsSqlContainer.DisposeAsync().AsTask();
        else
            await _dbContainer.DisposeAsync().AsTask();

        await _redisContainer.DisposeAsync().AsTask();
    }
}

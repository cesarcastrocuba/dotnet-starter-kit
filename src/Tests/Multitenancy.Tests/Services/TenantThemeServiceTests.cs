using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Caching;
using FSH.Framework.Shared.Multitenancy;
using FSH.Framework.Storage.Services;
using FSH.Modules.Multitenancy.Data;
using FSH.Modules.Multitenancy.Domain;
using FSH.Modules.Multitenancy.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Multitenancy.Tests.Services;

public class TenantThemeServiceTests
{
    private sealed class TestAccessor : IMultiTenantContextAccessor<AppTenantInfo>
    {
        public IMultiTenantContext<AppTenantInfo> MultiTenantContext { get; set; } = null!;
        IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => MultiTenantContext;
    }

    [Fact]
    public async Task ResetThemeAsync_ShouldInvalidateDefaultThemeCache()
    {
        // Arrange
        var cache = Substitute.For<ITenantCacheService>();

        // Use SQLite in-memory for testing to follow project rules and avoid InMemoryDatabase issues
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlite(connection)
            .Options;

        // Use a simple test accessor class to avoid NSubstitute/EF Core expression issues
        var tenantAccessor = new TestAccessor();
        var tenantInfo = new AppTenantInfo("test-tenant", "Test", null, "test@test.com", null);
        tenantAccessor.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantInfo);

        using (var dbContext = new TenantDbContext(options, tenantAccessor))
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        // We use a fresh context to ensure no caching issues from seed
        using (var dbContext = new TenantDbContext(options, tenantAccessor))
        {
            // Seed a theme bypassing the service
            var theme = TenantTheme.Create("test-tenant");
            dbContext.TenantThemes.Add(theme);
            await dbContext.SaveChangesAsync();
        }

        using (var dbContext = new TenantDbContext(options, tenantAccessor))
        {
            var storageService = Substitute.For<IStorageService>();
            var logger = Substitute.For<ILogger<TenantThemeService>>();
            var service = new TenantThemeService(cache, dbContext, tenantAccessor, storageService, logger);

            // Act
            await service.ResetThemeAsync("test-tenant", CancellationToken.None);

            // Assert: DefaultThemeCacheKey ("theme:default") was invalidated via ITenantCacheService
            await cache.Received(1).RemoveAsync("theme:default", Arg.Any<CancellationToken>());
        }
    }
}

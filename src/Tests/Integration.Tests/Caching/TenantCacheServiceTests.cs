using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Caching;
using FSH.Framework.Shared.Multitenancy;
using FSH.Tests.Integration.Infrastructure;
using FSH.Tests.Shared.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FSH.Tests.Integration.Caching;

[Collection("Integration")]
public class TenantCacheServiceTests : BaseIntegrationTest
{
    public TenantCacheServiceTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task CacheKeys_ShouldBeIsolated_PerTenant()
    {
        var tenantA = new AppTenantInfo { Id = "tenant-a", Identifier = "tenant-a", Name = "Tenant A" };
        var tenantB = new AppTenantInfo { Id = "tenant-b", Identifier = "tenant-b", Name = "Tenant B" };

        var key = "test-key";
        var valueA = "Value for Tenant A";
        var valueB = "Value for Tenant B";

        // Setup Tenant A
        using (var scopeA = Factory.Services.CreateScope())
        {
            var setter = scopeA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            var tenantCache = scopeA.ServiceProvider.GetRequiredService<ITenantCacheService>();
            await tenantCache.GetOrSetAsync<string>(key, () => Task.FromResult<string?>(valueA));
        }

        // Setup Tenant B
        using (var scopeB = Factory.Services.CreateScope())
        {
            var setter = scopeB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            var tenantCache = scopeB.ServiceProvider.GetRequiredService<ITenantCacheService>();
            await tenantCache.GetOrSetAsync<string>(key, () => Task.FromResult<string?>(valueB));
        }

        // Verify Tenant A still has its value
        using (var scopeVerifyA = Factory.Services.CreateScope())
        {
            var setter = scopeVerifyA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            var tenantCache = scopeVerifyA.ServiceProvider.GetRequiredService<ITenantCacheService>();
            var cachedValue = await tenantCache.GetAsync<string>(key);
            
            cachedValue.ShouldBe(valueA);
        }

        // Verify Tenant B still has its value
        using (var scopeVerifyB = Factory.Services.CreateScope())
        {
            var setter = scopeVerifyB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setter.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            var tenantCache = scopeVerifyB.ServiceProvider.GetRequiredService<ITenantCacheService>();
            var cachedValue = await tenantCache.GetAsync<string>(key);
            
            cachedValue.ShouldBe(valueB);
        }
    }
}

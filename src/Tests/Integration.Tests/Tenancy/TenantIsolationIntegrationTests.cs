using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Domain;
using FSH.Modules.Multitenancy.Data;
using FSH.Tests.Integration.Infrastructure;
using FSH.Tests.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FSH.Tests.Integration.Tenancy;

public class TenantIsolationIntegrationTests : BaseIntegrationTest
{
    public TenantIsolationIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task TenantData_ShouldBeIsolated_WhenUsingDifferentTenants()
    {
        // Arrange: Create data for Tenant A
        var tenantA = new AppTenantInfo { Id = "tenant-a", Identifier = "tenant-a", Name = "Tenant A" };
        var themeA = TenantTheme.Create(tenantA.Id);
        themeA.PrimaryColor = "#AAAAAA";

        // Arrange: Create data for Tenant B
        var tenantB = new AppTenantInfo { Id = "tenant-b", Identifier = "tenant-b", Name = "Tenant B" };
        var themeB = TenantTheme.Create(tenantB.Id);
        themeB.PrimaryColor = "#BBBBBB";

        // Act: Save Tenant A data
        using (var scopeA = Factory.Services.CreateScope())
        {
            var setterA = scopeA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterA.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            var dbContextA = scopeA.ServiceProvider.GetRequiredService<TenantDbContext>();
            
            dbContextA.TenantThemes.Add(themeA);
            await dbContextA.SaveChangesAsync();
        }

        // Act: Save Tenant B data
        using (var scopeB = Factory.Services.CreateScope())
        {
            var setterB = scopeB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterB.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            var dbContextB = scopeB.ServiceProvider.GetRequiredService<TenantDbContext>();
            
            dbContextB.TenantThemes.Add(themeB);
            await dbContextB.SaveChangesAsync();
        }

        // Act: Verify Tenant A isolation
        using (var scopeVerifyA = Factory.Services.CreateScope())
        {
            var setterVerifyA = scopeVerifyA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterVerifyA.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            var dbContextVerifyA = scopeVerifyA.ServiceProvider.GetRequiredService<TenantDbContext>();
            
            var themesA = await dbContextVerifyA.TenantThemes.ToListAsync();
            themesA.Count.ShouldBe(1);
            themesA[0].TenantId.ShouldBe(tenantA.Id);
            themesA[0].PrimaryColor.ShouldBe("#AAAAAA");
        }

        // Act: Verify Tenant B isolation
        using (var scopeVerifyB = Factory.Services.CreateScope())
        {
            var setterVerifyB = scopeVerifyB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterVerifyB.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            var dbContextVerifyB = scopeVerifyB.ServiceProvider.GetRequiredService<TenantDbContext>();
            
            var themesB = await dbContextVerifyB.TenantThemes.ToListAsync();
            themesB.Count.ShouldBe(1);
            themesB[0].TenantId.ShouldBe(tenantB.Id);
            themesB[0].PrimaryColor.ShouldBe("#BBBBBB");
        }
    }
}

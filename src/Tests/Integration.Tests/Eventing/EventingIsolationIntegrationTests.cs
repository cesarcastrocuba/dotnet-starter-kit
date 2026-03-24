using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Eventing.Inbox;
using FSH.Framework.Eventing.Outbox;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Identity.Data;
using FSH.Tests.Integration.Infrastructure;
using FSH.Tests.Shared.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace FSH.Tests.Integration.Eventing;

public class EventingIsolationIntegrationTests : BaseIntegrationTest
{
    public EventingIsolationIntegrationTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task EventingData_ShouldBeIsolated_WhenUsingDifferentTenants()
    {
        // Arrange: Create data for Tenant A
        var tenantA = new AppTenantInfo { Id = "tenant-a", Identifier = "tenant-a", Name = "Tenant A" };
        var outboxA = new OutboxMessage 
        { 
            Id = Guid.NewGuid(), 
            Type = "TestEvent", 
            Payload = "{}", 
            TenantId = tenantA.Id,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            CorrelationId = Guid.NewGuid().ToString()
        };

        // Arrange: Create data for Tenant B
        var tenantB = new AppTenantInfo { Id = "tenant-b", Identifier = "tenant-b", Name = "Tenant B" };
        var outboxB = new OutboxMessage 
        { 
            Id = Guid.NewGuid(), 
            Type = "TestEvent", 
            Payload = "{}", 
            TenantId = tenantB.Id,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            CorrelationId = Guid.NewGuid().ToString()
        };

        // Act: Save Tenant A data in its own scope
        using (var scopeA = Factory.Services.CreateScope())
        {
            var setterA = scopeA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterA.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            // Resolve dbContext AFTER setting context
            var dbContextA = scopeA.ServiceProvider.GetRequiredService<IdentityDbContext>();
            
            dbContextA.OutboxMessages.Add(outboxA);
            await dbContextA.SaveChangesAsync();
        }

        // Act: Save Tenant B data in its own scope
        using (var scopeB = Factory.Services.CreateScope())
        {
            var setterB = scopeB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterB.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            // Resolve dbContext AFTER setting context
            var dbContextB = scopeB.ServiceProvider.GetRequiredService<IdentityDbContext>();
            
            dbContextB.OutboxMessages.Add(outboxB);
            await dbContextB.SaveChangesAsync();
        }

        // Act: Verify Tenant A data isolation
        using (var scopeVerifyA = Factory.Services.CreateScope())
        {
            var setterVerifyA = scopeVerifyA.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterVerifyA.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantA);
            
            var dbContextVerifyA = scopeVerifyA.ServiceProvider.GetRequiredService<IdentityDbContext>();
            
            var messagesA = await dbContextVerifyA.OutboxMessages.ToListAsync();
            messagesA.Count.ShouldBe(1);
            messagesA[0].TenantId.ShouldBe(tenantA.Id);
            messagesA[0].CorrelationId.ShouldBe(outboxA.CorrelationId);
        }

        // Act: Verify Tenant B data isolation
        using (var scopeVerifyB = Factory.Services.CreateScope())
        {
            var setterVerifyB = scopeVerifyB.ServiceProvider.GetRequiredService<IMultiTenantContextSetter>();
            setterVerifyB.MultiTenantContext = new MultiTenantContext<AppTenantInfo>(tenantB);
            
            var dbContextVerifyB = scopeVerifyB.ServiceProvider.GetRequiredService<IdentityDbContext>();
            
            var messagesB = await dbContextVerifyB.OutboxMessages.ToListAsync();
            messagesB.Count.ShouldBe(1);
            messagesB[0].TenantId.ShouldBe(tenantB.Id);
            messagesB[0].CorrelationId.ShouldBe(outboxB.CorrelationId);
        }
    }
}

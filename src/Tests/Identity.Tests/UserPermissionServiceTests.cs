using FSH.Framework.Caching;
using FSH.Modules.Identity.Domain;
using FSH.Modules.Identity.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

namespace Identity.Tests.Services;

public class UserPermissionServiceTests
{
    [Fact]
    public async Task InvalidatePermissionCacheAsync_ShouldScopeKeyToTenant()
    {
        // Arrange
        var userStore = Substitute.For<IUserStore<FshUser>>();
        var userManager = Substitute.For<UserManager<FshUser>>(userStore, null, null, null, null, null, null, null, null);

        var roleStore = Substitute.For<IRoleStore<FshRole>>();
        var roleManager = Substitute.For<RoleManager<FshRole>>(roleStore, null, null, null, null);

        var tenantCache = Substitute.For<ITenantCacheService>();

        var service = new UserPermissionService(userManager, roleManager, null!, tenantCache);

        // Act
        await service.InvalidatePermissionCacheAsync("user-1", CancellationToken.None);

        // Assert: The key no longer includes tenantId (tenantId is now injected by TenantCacheService)
        await tenantCache.Received(1).RemoveAsync("perm:user-1", Arg.Any<CancellationToken>());
    }
}

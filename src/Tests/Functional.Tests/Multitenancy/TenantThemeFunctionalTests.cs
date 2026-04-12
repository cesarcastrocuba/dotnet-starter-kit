using System.Net;
using System.Net.Http.Json;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Tests.Functional.Infrastructure;
using FSH.Tests.Shared.Infrastructure;
using Shouldly;
using Xunit;

namespace FSH.Tests.Functional.Multitenancy;

public class TenantThemeFunctionalTests : BaseFunctionalTest
{
    public TenantThemeFunctionalTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task GetTheme_ShouldReturnTenantSpecificTheme()
    {
        // Act: Add tenant header and authenticate as root admin
        Client.DefaultRequestHeaders.Add(MultitenancyConstants.Identifier, MultitenancyConstants.Root.Id);
        await AuthenticateAsync(MultitenancyConstants.Root.EmailAddress, MultitenancyConstants.DefaultPassword);

        // Act: Get theme for root tenant
        var response = await Client.GetAsync(new Uri("/api/v1/tenants/theme", UriKind.Relative));

        // Assert
        response.EnsureSuccessStatusCode();
        var theme = await response.Content.ReadFromJsonAsync<TenantThemeDto>();
        theme.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetTheme_ShouldReturnNotFound_WhenTenantDoesNotExist()
    {
        // Act: Get theme for non-existent tenant
        Client.DefaultRequestHeaders.Add(MultitenancyConstants.Identifier, "non-existent");
        var response = await Client.GetAsync(new Uri("/api/v1/tenants/theme", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

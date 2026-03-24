using System.Net;
using System.Net.Http.Json;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Tests.Functional.Infrastructure;
using FSH.Tests.Shared.Infrastructure;
using Shouldly;
using Xunit;

namespace FSH.Tests.Functional.Multitenancy;

public class TenantProvisioningFunctionalTests : BaseFunctionalTest
{
    public TenantProvisioningFunctionalTests(CustomWebApplicationFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task GetProvisioningStatus_ShouldReturnStatus_WhenTenantExists()
    {
        // Act: Add tenant header and authenticate as root admin
        Client.DefaultRequestHeaders.Add(MultitenancyConstants.Identifier, MultitenancyConstants.Root.Id);
        await AuthenticateAsync(MultitenancyConstants.Root.EmailAddress, MultitenancyConstants.DefaultPassword);

        // Act 1: Create a new tenant to generate a provisioning record
        var command = new FSH.Modules.Multitenancy.Contracts.v1.CreateTenant.CreateTenantCommand("test-prov-tenant", "Test Prov Tenant", null, "admin@testprov.com", null);
        var createResponse = await Client.PostAsJsonAsync("/api/v1/tenants", command);
        if (!createResponse.IsSuccessStatusCode)
        {
            var createError = await createResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Create Tenant Failed: {createResponse.StatusCode}. Output: {createError}");
        }

        // Act 2: Get provisioning status for the new tenant
        var response = await Client.GetAsync(new Uri($"/api/v1/tenants/test-prov-tenant/provisioning", UriKind.Relative));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed with status {response.StatusCode}. Output: {error}");
        }

        // Assert
        response.EnsureSuccessStatusCode();
        var status = await response.Content.ReadFromJsonAsync<TenantProvisioningStatusDto>();
        status.ShouldNotBeNull();
        status.TenantId.ShouldBe("test-prov-tenant");
    }

    [Fact]
    public async Task GetProvisioningStatus_ShouldReturnNotFound_WhenTenantDoesNotExist()
    {
        // Act: Authenticate as root admin, because viewing provisioning status requires permissions
        Client.DefaultRequestHeaders.Add(MultitenancyConstants.Identifier, MultitenancyConstants.Root.Id);
        await AuthenticateAsync(MultitenancyConstants.Root.EmailAddress, MultitenancyConstants.DefaultPassword);

        // Act: Get provisioning status for non-existent tenant
        var response = await Client.GetAsync(new Uri("/api/v1/tenants/non-existent/provisioning", UriKind.Relative));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}

using FSH.Framework.Shared.Persistence;
using FSH.Modules.Multitenancy.Contracts.Dtos;
using FSH.Modules.Multitenancy.Contracts.v1.GetTenants;
using FSH.Framework.Shared.Multitenancy;

namespace FSH.Modules.Multitenancy.Contracts;

public interface ITenantService
{
    ValueTask<PagedResponse<TenantDto>> GetAllAsync(GetTenantsQuery query, CancellationToken cancellationToken);

    ValueTask<bool> ExistsWithIdAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken = default);

    ValueTask<TenantStatusDto> GetStatusAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<string> CreateAsync(string id, string name, string? connectionString, string adminEmail, string? issuer, CancellationToken cancellationToken);

    ValueTask<string> ActivateAsync(string id, CancellationToken cancellationToken);

    ValueTask<string> DeactivateAsync(string id, CancellationToken cancellationToken = default);

    ValueTask<DateTimeOffset> UpgradeSubscriptionAsync(string id, DateTimeOffset extendedExpiryDate, CancellationToken cancellationToken = default);

    ValueTask MigrateTenantAsync(AppTenantInfo tenant, CancellationToken cancellationToken);

    ValueTask SeedTenantAsync(AppTenantInfo tenant, CancellationToken cancellationToken);
}

namespace FSH.Framework.Caching;

/// <summary>
/// Tenant-scoped cache service. All keys are automatically prefixed with the current tenant ID.
/// Resulting key format: "{tenantId}:{key}"
/// </summary>
public interface ITenantCacheService
{
    /// <summary>Gets or sets an item in the tenant-scoped cache.</summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? sliding = null, CancellationToken ct = default);

    /// <summary>Gets an item from the tenant-scoped cache.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    /// <summary>Removes an item from the tenant-scoped cache.</summary>
    Task RemoveAsync(string key, CancellationToken ct = default);
}

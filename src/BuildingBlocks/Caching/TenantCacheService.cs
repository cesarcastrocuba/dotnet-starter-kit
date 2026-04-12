using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Shared.Multitenancy;

namespace FSH.Framework.Caching;

/// <summary>
/// Wraps ICacheService and automatically prefixes all keys with the current tenant ID.
/// </summary>
public sealed class TenantCacheService : ITenantCacheService
{
    private readonly ICacheService _cache;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantAccessor;

    public TenantCacheService(
        ICacheService cache,
        IMultiTenantContextAccessor<AppTenantInfo> tenantAccessor)
    {
        _cache = cache;
        _tenantAccessor = tenantAccessor;
    }

    private string ScopedKey(string key)
    {
        var tenantId = _tenantAccessor.MultiTenantContext?.TenantInfo?.Id 
                       ?? throw new InvalidOperationException("No tenant context available for tenant-scoped cache.");
        return $"{tenantId}:{key}";
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? sliding = null, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var scopedKey = ScopedKey(key);
        var cached = await _cache.GetItemAsync<T>(scopedKey, ct).ConfigureAwait(false);
        if (cached is not null) return cached;

        var value = await factory().ConfigureAwait(false);
        if (value is not null)
            await _cache.SetItemAsync(scopedKey, value, sliding, ct).ConfigureAwait(false);

        return value;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) => _cache.GetItemAsync<T>(ScopedKey(key), ct);

    public Task RemoveAsync(string key, CancellationToken ct = default) => _cache.RemoveItemAsync(ScopedKey(key), ct);
}

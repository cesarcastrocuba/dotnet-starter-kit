# Walkthrough: [TIER 1] fix/tenant-cache-service - ITenantCacheService structural guardrail

We identified that `ICacheService` had no tenant isolation — any developer injecting it directly in a business module could inadvertently create cross-tenant cache leaks. We solved this by introducing `ITenantCacheService`, a strongly-typed wrapper that automatically scopes all cache keys to the current tenant context, making the safe path the default path.

## 1. Key Changes

| File | Change |
|---|---|
| `BuildingBlocks/Caching/ITenantCacheService.cs` | New interface (`GetOrSetAsync`, `GetAsync`, `RemoveAsync`) |
| `BuildingBlocks/Caching/TenantCacheService.cs` | Impl — wraps `ICacheService`, keys as `{tenantId}:{key}` |
| `BuildingBlocks/Caching/Extensions.cs` | Registers `ITenantCacheService` as `Scoped` (both Redis & in-memory paths) |
| `Modules.Identity/Services/UserPermissionService.cs` | Refactored to inject `ITenantCacheService` |
| `Modules.Multitenancy/Services/TenantThemeService.cs` | Refactored to inject `ITenantCacheService` |
| `Architecture.Tests/CachingGuardrailTests.cs` | New structural test blocking direct `ICacheService` usage in modules |
| `Integration.Tests/Caching/TenantCacheServiceTests.cs` | New integration test proving tenant cache isolation |

## 2. Visual Evidence / Logs

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Passed!  - Failed: 0, Passed: 221 - Identity.Tests.dll
Passed!  - Failed: 0, Passed:  97 - Multitenancy.Tests.dll
Passed!  - Failed: 0, Passed:  51 - Architecture.Tests.dll
Passed!  - Failed: 0, Passed:   4 - Integration.Tests.dll
Passed!  - Failed: 0, Passed:   5 - Functional.Tests.dll
Passed!  - Failed: 0, Passed:  61 - Auditing.Tests.dll
Passed!  - Failed: 0, Passed:  51 - Generic.Tests.dll
Passed!  - Failed: 0, Passed:   1 - Spec.Tests.dll

Total: 491 passed, 0 failed
```

## 3. Key Learnings & Technical Debt
- `Caching` BB now depends on `Shared` (for `AppTenantInfo`). This is a deliberate and minimal coupling, updated in the architecture test documentation accordingly.
- `DefaultThemeCacheKey` in `TenantThemeService` was a global, shared key before. Now it's automatically tenant-scoped, so the default theme is per-tenant when using `ITenantCacheService`. This is the correct behavior.

## 4. Deployment Notes
- No database migrations required.
- `ITenantCacheService` is registered as `Scoped`, which is required because it depends on `IMultiTenantContextAccessor` (also request-scoped). Do **not** attempt to inject it in Singleton services.
- Branch `fix/tenant-cache-service` is ready to merge into `develop`.

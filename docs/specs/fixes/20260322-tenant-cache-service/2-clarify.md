# Clarifications: [TIER 1] fix/tenant-cache-service - ITenantCacheService structural guardrail

## Unresolved Questions

1. **Location of `ITenantCacheService`**: 
   Should `ITenantCacheService` be added to `BuildingBlocks/Caching`? If so, does `BuildingBlocks/Caching` have access to the current tenant ID (e.g., via `Finbuckle.MultiTenant.Abstractions.IMultiTenantContextAccessor`), or should the tenant ID be passed via another context (like an `ICurrentUser` or similar from `BuildingBlocks/Core`)?

2. **Key Format**: 
   Inside `ITenantCacheService`, should the cache key simply be prefixed with the `TenantId` (e.g., `"{tenantId}:{originalKey}"`) before calling the underlying `ICacheService`? 

3. **Current Usages Migration**: 
   I found that `ICacheService` is currently used in at least two places inside business modules:
   - `TenantThemeService` (Multitenancy module)
   - `UserPermissionService` (Identity module)
   Should we refactor both to use `ITenantCacheService`?

4. **Architecture Test Enforcement**:
   To ensure this structural guardrail is strictly followed, should I write an architecture test in `Architecture.Tests` that asserts `ICacheService` cannot be referenced by classes inside `src/Modules/` (forcing them to use `ITenantCacheService` instead)?

## Decisions Made

1. **Location**: `ITenantCacheService` will be added to `BuildingBlocks/Caching`. It will rely on an existing tenant abstraction from `BuildingBlocks/Core` (e.g., `Finbuckle.MultiTenant.ITenantInfo` or similar).
2. **Key Format**: The key will be explicitly formatted via string interpolation as `$"tenant:{tenantId}:{key}"` inside the `ITenantCacheService` wrapper.
3. **Current Usages**: Both `TenantThemeService` and `UserPermissionService` will be refactored to use `ITenantCacheService` instead of `ICacheService`.
4. **Architecture Test Enforcement**: An architecture test will be added to strictly forbid `ICacheService` from being constructor-injected in any class within `src/Modules/`.

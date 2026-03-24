# Technical Plan: [TIER 1] fix/tenant-cache-service - ITenantCacheService structural guardrail

## Architecture & Design
This feature aims to prevent cross-tenant cache contamination by introducing `ITenantCacheService`. It will act as a wrapper around the existing global `ICacheService`, transparently injecting the current tenant's ID into every cache key (e.g., `tenant:{tenantId}:{key}`). We will implement this interface in `BuildingBlocks/Caching` relying on the core tenant context. To ensure this structural guardrail is respected, an architecture test will enforce its use across all business modules by banning the direct injection of `ICacheService`.

## Proposed Changes (File Level)

### BuildingBlocks/Caching
- `[NEW] src/BuildingBlocks/Caching/ITenantCacheService.cs`: Interface identical to `ICacheService` (or providing the same essential methods), where the tenant context is implicit.
- `[NEW] src/BuildingBlocks/Caching/TenantCacheService.cs`: Implementation that wraps `ICacheService`, retrieves the current `TenantId` (e.g., from `Finbuckle.MultiTenant.ITenantInfo`), and prefixes the cache keys.
- `[MODIFY] src/BuildingBlocks/Caching/Extensions.cs`: Register `ITenantCacheService` and its implementation in DI (Scoped/Transient based on the tenant context lifecycle).

### Modules/Identity & Modules/Multitenancy
- `[MODIFY] src/Modules/Multitenancy/Modules.Multitenancy/Services/TenantThemeService.cs`: Replace `ICacheService` with `ITenantCacheService`.
- `[MODIFY] src/Modules/Identity/Modules.Identity/Services/UserPermissionService.cs`: Replace `ICacheService` with `ITenantCacheService`.

### Tests/Architecture.Tests
- `[MODIFY] src/Tests/Architecture.Tests/BuildingBlocksIndependenceTests.cs` (or equivalent file): Add test `BusinessModules_ShouldNot_DependOn_ICacheService_Directly` to assert that no class in `src/Modules/` depends directly on `ICacheService`.

## Testing Strategy
- **Integration Specs**: Create `src/Tests/Integration.Tests/Caching/TenantCacheServiceTests.cs` to verify cache isolation at the service level (storing a value in one tenant's cache does not leak to another tenant).
- **Functional Tests**: Verify (or add) functional tests for the endpoints that use `TenantThemeService` or `UserPermissionService` to ensure the end-to-end API response is properly scoped by the `X-Tenant` header without caching cross-contamination.
- **Unit Tests**: Test `TenantCacheService` to verify that methods like `SetAsync` and `GetAsync` prepend `$"tenant:{tenantId}:"` correctly to the key before calling the inner `ICacheService`.
- **Architecture Tests**: Ensure `ICacheService` usage is blocked structurally in `src/Modules/`.
- **Manual Verification**: Run `dotnet build src/FSH.Framework.slnx` and `dotnet test src/FSH.Framework.slnx` to ensure 0 errors and 0 warnings in both build and test execution.

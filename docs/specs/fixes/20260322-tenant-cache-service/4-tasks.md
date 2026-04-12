# Implementation Tasks: [TIER 1] fix/tenant-cache-service

## 1. Test Setup (Red)
- [ ] Create `src/Tests/Integration.Tests/Caching/TenantCacheServiceTests.cs` testing cache isolation between tenants.
- [ ] Create/Update unit tests for `TenantCacheService` to verify the `$"tenant:{tenantId}:"` prefix logic.
- [ ] Create architecture test `BusinessModules_ShouldNot_DependOn_ICacheService_Directly` in `src/Tests/Architecture.Tests`.

## 2. Implementation (Green)
- [ ] Create `src/BuildingBlocks/Caching/ITenantCacheService.cs` with `GetOrSetAsync`, `GetAsync`, and `RemoveAsync` (CACHE-3).
- [ ] Create `src/BuildingBlocks/Caching/TenantCacheService.cs` implementing `ITenantCacheService`. Inject `IMultiTenantContextAccessor<AppTenantInfo>` and ensure `ScopedKey()` throws `InvalidOperationException` if tenant is missing (CACHE-4).
- [ ] Update `src/BuildingBlocks/Caching/CacheServiceExtensions.cs` (or `Extensions.cs`) to register `ITenantCacheService` as `Scoped` (CACHE-5).
- [ ] Refactor `src/Modules/Multitenancy/Modules.Multitenancy/Services/TenantThemeService.cs` to inject `ITenantCacheService`.
- [ ] Refactor `src/Modules/Identity/Modules.Identity/Services/UserPermissionService.cs` to inject `ITenantCacheService`.

## 3. Verification & Polish
- [ ] Ensure all local tests pass (`dotnet test src/FSH.Framework.slnx`) with 0 errors and 0 warnings.
- [ ] Ensure `dotnet build src/FSH.Framework.slnx` succeeds with 0 errors and 0 warnings.
- [ ] Verify functional tests for affected endpoints pass successfully.

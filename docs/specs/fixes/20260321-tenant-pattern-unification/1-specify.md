# Specification: Tenant Pattern Unification (#11)

## 1. Description
Unify the multi-tenancy pattern across the modular monolith by ensuring all per-tenant entities are correctly configured with Finbuckle's `.IsMultiTenant()`. This prevents data leakage and ensures consistent isolation.

## 2. Requirements & User Stories
- **Requirement 1**: Shared `OutboxMessage` and `InboxMessage` entities in `BuildingBlocks/Eventing` must be marked as multi-tenant.
- **Requirement 2**: Theme and provisioning entities in the `Multitenancy` module must be marked as multi-tenant.
- **Requirement 3**: Automated guardrails (Architecture Tests) must prevent future entities from missing `.IsMultiTenant()` if they are intended to be isolated.
- **Requirement 4**: Unit & Integration tests must verify that `TenantTheme`, `TenantProvisioning`, and `TenantProvisioningStep` are correctly filtered by tenant in the database context.
- **Requirement 5**: Functional tests must verify that API endpoints for multitenancy features respect tenant isolation.

## 3. Acceptance Criteria
- [ ] `OutboxMessageConfiguration` calls `.IsMultiTenant()`.
- [ ] `InboxMessageConfiguration` calls `.IsMultiTenant()`.
- [ ] `TenantThemeConfiguration` calls `.IsMultiTenant()`.
- [ ] `TenantProvisioningConfiguration` calls `.IsMultiTenant()`.
- [ ] `TenantProvisioningStepConfiguration` calls `.IsMultiTenant()`.
- [ ] New architecture test `TenancyIsolationTests` verifies `IHasTenant` entities have multi-tenancy configuration.
- [ ] New integration tests verify that querying `TenantTheme` returns only data for the current tenant.
- [ ] New functional tests verify that endpoints restricted to a tenant return only that tenant's configuration.
- [ ] Build has 0 warnings.
- [ ] All tests pass.

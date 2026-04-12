# Specification: Tenancy Migration & Standardization

## 1. Description
This specification outlines the second phase of Tenancy Isolation fixes and project-wide timestamp standardization. It focuses on transitioning all `DateTime` properties to `DateTimeOffset` with a unified `OnUtc` naming convention, enforcing stronger tenancy isolation in the Identity module, and organizing migration history.

## 2. Requirements & User Stories
### Schema & Performance (Issue #7)
- **SCHEMA-1**: Ensure an explicit non-clustered unique index exists on `TenantTheme.TenantId`.
- **SCHEMA-2**: Move all Multitenancy tables to the `tenant` schema in `TenantDbContext`.
- **EVENTING-2b**: Include `TenantId` in the composite primary key of `InboxMessage` to allow cross-tenant idempotency.
- **AUDIT-5**: Standardize `LastModifiedOnUtc` and `LastModifiedBy` handling across all auditable entities.

### Standardization (Issue #7 + Project-Wide)
- **SCHEMA-3**: Standardize ALL timestamp properties to `DateTimeOffset` and rename them using the `OnUtc` suffix (e.g., `CreatedOnUtc`, `ExpiresOnUtc`, `OccurredOnUtc`, `ReceivedOnUtc`).
- **TENANT-1**: Ensure `TenantId` is exactly 64 characters across all modules and infrastructure (Audit, Eventing, Identity).
- **CONVENTIONS**: Remove legacy database column aliases (`HasColumnName`) to align schema with C# property names.

### Identity Isolation (Reverted from Issue #6)
- **IDENTITY-1**: Add `.IsMultiTenant()` to `UserSession` and `PasswordHistory` configurations.
- **IDENTITY-2**: Ensure `FshUser` and other Identity entities follow the standardized timestamp convention.

## 3. Acceptance Criteria
- [x] All `DateTime` properties converted to `DateTimeOffset`.
- [x] All timestamp names standardized to `*OnUtc`.
- [x] `TenantDbContext` uses the `tenant` schema.
- [x] `InboxMessage` PK is `(Id, HandlerName, TenantId)`.
- [x] `UserSession` and `PasswordHistory` have explicit `IsMultiTenant()` configuration.
- [x] Legacy `Audit` migrations consolidated into the `Auditing` folder with matching namespaces.
- [x] All 378 tests (Identity, Multitenancy, Auditing) pass with 100% success rate.
- [x] Zero build warnings in the final solution.

# Technical Plan: Tenancy Migration & Standardization

## Architecture & Design
This plan enforces project-wide consistency for tenancy isolation and temporal data representation.

1.  **Strict Tenancy**: Explicit isolation and length constraints for all Identity, Eventing, and Audit entities.
2.  **Temporal Standardization**: Converge on `DateTimeOffset` and `OnUtc` naming for all timestamps.
3.  **Schema Organization**: Group infrastructure tables into schemas (`tenant`, `audit`, etc.) and organize migrations by module.

## Implemented Changes

### Core Standardization
- **Naming**: Renamed all timestamp properties to follow the `OnUtc` pattern.
- **Types**: Converted all `DateTime` timestamps to `DateTimeOffset`.
- **Infrastructure**: Updated `OutboxMessage`, `InboxMessage`, `AuditRecord`, and `TenantProvisioning`.
- **Contracts**: Updated all DTOs and Interfaces (`TokenResponse`, `TenantStatusDto`, `AuditSummaryDto`, etc.).

### [Modules.Identity]
- **Standardized Entities**: `FshUser`, `UserSession`, `PasswordHistory`, `GroupRole`, `UserGroup`.
- **Services**: `TokenService`, `SessionService`, `IdentityService`, `PasswordExpiryService`.
- **Tests**: Comprehensive updates to `GenerateTokenCommandHandlerTests` and `PasswordExpiryServiceTests` to handle `DateTimeOffset` and `ValueTask` return types correctly.

### [Modules.Multitenancy]
- **TenantDbContext**: Moved to `tenant` schema.
- **TenantTheme**: Optimized unikely index and auditing logic.
- **Provisioning**: Converted all lifecycle timestamps to `OnUtc`.

### [Modules.Auditing]
- **AuditRecord**: Standardized properties and removed legacy "text/64" comments.
- **AuditEnvelope**: Implemented `.ToUniversalTime()` in constructor to ensure UTC consistency.
- **Migration**: Moved legacy `Audit` folder migrations to `Auditing` and updated namespaces.

## Migration Strategy
Migrations generated in `src/Playground/Migrations.PostgreSQL`:
1.  **Identity**: `20260319_StandardizeIdentityTimestamps`
2.  **MultiTenancy**: `20260319_StandardizeMultitenancyTimestamps`
3.  **Auditing**: `20260319_StandardizeAuditingTimestamps` (Consolidated history).

## Testing Strategy (Finalized)
- **Unit Tests**: Updated to reflect `OnUtc` naming and `DateTimeOffset` types.
- **Mocking**: Fixed `NSubstitute` issues with `ValueTask` returns by using proper async setup.
- **Verification**: Executed 378 tests across Auditing, Identity, and Multitenancy with 100% pass rate.

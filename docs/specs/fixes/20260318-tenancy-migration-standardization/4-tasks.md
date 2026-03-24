# Implementation Tasks: Tenancy Migration & Standardization

## 1. Entity & Domain Updates
- [x] Standardize ALL timestamp properties to `DateTimeOffset` and rename to `*OnUtc`.
- [x] Update Identity Module (FshUser, UserSession, PasswordHistory, Group, Role).
- [x] Update Auditing Module (AuditRecord, IAuditEvent, AuditEnvelope).
- [x] Update Multitenancy Module (AppTenantInfo, TenantProvisioning, TenantTheme).
- [x] Resolve `TenantId` length (64) across all infrastructure and domain entities.

## 2. DTOs, Services & Contracts
- [x] Standardize `TokenResponse` and `RefreshTokenCommandResponse`.
- [x] Standardize Tenant DTOs (`TenantDto`, `TenantStatusDto`, `TenantLifecycleResultDto`).
- [x] Standardize Audit DTOs (`AuditSummaryDto`, `AuditDetailDto`).
- [x] Update `ITokenService`, `ITenantService`, and `ISessionService` interfaces.

## 3. EF Core Configurations & Migrations
- [x] Update all `Identity` configurations (call `.IsMultiTenant()` and set `TenantId` to 64).
- [x] Update `TenantDbContext.cs` to use `HasDefaultSchema("tenant")`.
- [x] Update `InboxMessage` and `OutboxMessage` configurations.
- [x] Remove legacy `HasColumnName` aliases across all modules.
- [x] Consolidation: Move legacy `Audit` migrations to `Auditing` and update namespaces.
- [x] Generate standardized migrations (Identity, MultiTenancy, Auditing).

## 4. Test Updates & Verification
- [x] Update `GenerateTokenCommandHandlerTests` (Mocking & Naming).
- [x] Update `PasswordExpiryServiceTests` (ValueTask & Naming).
- [x] Update `TenantProvisioningTests` and `TenantThemeTests`.
- [x] Update `ChangeTenantActivationCommandHandlerTests` (Naming).
- [x] Update `AuditEnvelopeTests` (UTC Conversion).
- [x] Fix `DateRangeValidatorTests` (Standardized Types & Messages).
- [x] Verify total success of **618 tests** (Architecture, Unit, Integration, Functional).

## 5. Final Debugging & Client officialization
- [x] Resolve 30 build errors in `Audits.razor` and `Generated.cs` (legacy `fromUtc`/`toUtc`).
- [x] Create and apply `Identity_TemporalStandardization_Fix` migration.
- [x] Create and apply `Multitenancy_ValidUptoStandardization` migration.
- [x] Regenerate `ApiClient/Generated.cs` using the official `generate-api-clients.ps1` script (with running Aspire API).
- [x] Achieve **0 build warnings** across the entire solution.

## 6. Final Review & Documentation
- [x] Clean up legacy comments in `AuditRecord.cs`.
- [x] Complete final `walkthrough.md` and SDD documentation update.

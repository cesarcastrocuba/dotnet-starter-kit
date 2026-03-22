# Technical Plan: Tenant Pattern Unification (#11)

## Architecture & Design
In the FSH .NET Starter Kit, multi-tenancy is handled via Finbuckle. Entities that belong to a tenant should implement `IHasTenant` and their EF Core configuration must call `.IsMultiTenant()`.
This plan unifies the pattern by ensuring that shared infrastructure entities (Outbox/Inbox) and module-specific entities (Themes/Provisioning) are correctly isolated.

## Proposed Changes (File Level)

### [BuildingBlock] Eventing
- `src/BuildingBlocks/Eventing/Outbox/OutboxMessage.cs`: Add `.IsMultiTenant()` to `OutboxMessageConfiguration`.
- `src/BuildingBlocks/Eventing/Inbox/InboxMessage.cs`: Add `.IsMultiTenant()` to `InboxMessageConfiguration`.

### [Module] Multitenancy
- `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantThemeConfiguration.cs`: Add `.IsMultiTenant()`.
- `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantProvisioningConfiguration.cs`: Add `.IsMultiTenant()`.
- `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantProvisioningStepConfiguration.cs`: Add `.IsMultiTenant()`.

### [Architecture Tests]
- `src/Tests/Architecture.Tests/TenancyIsolationTests.cs`: [NEW] Verify all `IHasTenant` entities use `.IsMultiTenant()`.

### [Unit Tests] Logic & Domain
- `src/Tests/Multitenancy.Tests/Domain/TenantThemeTests.cs`: [UPDATE] Ensure complete coverage for theme initialization and tenant assignment.
- `src/Tests/Multitenancy.Tests/Domain/TenantProvisioningTests.cs`: [UPDATE] Ensure complete coverage for provisioning logic.

### [Integration Tests] Data Layer
- `src/Tests/Integration.Tests/Tenancy/TenantIsolationIntegrationTests.cs`: [NEW] Verify `TenantDbContext` applies Global Query Filters for `TenantTheme`, `TenantProvisioning`, and `TenantProvisioningStep`.
- `src/Tests/Integration.Tests/Eventing/EventingIsolationIntegrationTests.cs`: [NEW] Verify `OutboxMessage` and `InboxMessage` isolation in `BuildingBlocks`.

### [Functional Tests] API Layer
- `src/Tests/Functional.Tests/Multitenancy/TenantThemeFunctionalTests.cs`: [NEW] Verify API theme isolation using Testcontainers.
- `src/Tests/Functional.Tests/Multitenancy/TenantProvisioningFunctionalTests.cs`: [NEW] Verify API provisioning isolation using Testcontainers.

## Testing Strategy
- **Architecture**: `dotnet test src/Tests/Architecture.Tests`
- **Unit**: `dotnet test src/Tests/Multitenancy.Tests`
- **Integration**: `dotnet test src/Tests/Integration.Tests`
- **Functional**: `dotnet test src/Tests/Functional.Tests`
- **Build**: `dotnet build src/FSH.Framework.slnx` (Verify 0 warnings)

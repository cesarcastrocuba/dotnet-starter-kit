# Implementation Tasks: Tenant Pattern Unification (#11)

## 1. Test Setup (Red)
- [ ] Create `src/Tests/Architecture.Tests/TenancyIsolationTests.cs`.
- [ ] Update `src/Tests/Multitenancy.Tests/` unit tests for coverage.
- [ ] Create `src/Tests/Integration.Tests/Tenancy/TenantIsolationIntegrationTests.cs`.
- [ ] Create folder `src/Tests/Integration.Tests/Eventing/` and `EventingIsolationIntegrationTests.cs`.
- [ ] Create folder `src/Tests/Functional.Tests/Multitenancy/` and its test files.

## 2. Implementation (Green)
- [ ] Modify `src/BuildingBlocks/Eventing/Outbox/OutboxMessage.cs`: Add `.IsMultiTenant()` to `OutboxMessageConfiguration`.
- [ ] Modify `src/BuildingBlocks/Eventing/Inbox/InboxMessage.cs`: Add `.IsMultiTenant()` to `InboxMessageConfiguration`.
- [ ] Modify `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantThemeConfiguration.cs`: Add `.IsMultiTenant()`.
- [ ] Modify `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantProvisioningConfiguration.cs`: Add `.IsMultiTenant()`.
- [ ] Modify `src/Modules/Multitenancy/Modules.Multitenancy/Data/Configurations/TenantProvisioningStepConfiguration.cs`: Add `.IsMultiTenant()`.

## 3. Verification & Polish
- [ ] Run and pass Architecture tests.
- [ ] Run and pass Integration tests.
- [ ] Run and pass Functional tests.
- [ ] Ensure 0 build warnings (`dotnet build src/FSH.Framework.slnx`).
- [ ] Create implementation report (`5-implement.md`).
- [ ] Create walkthrough (`6-walkthrough.md`).

# Implementation Tasks: feat/eventing-domain-to-outbox-bridge — EVENTING-3/4/5

## 1. Test Setup (Red)
- [ ] **Unit**: `DomainEventToOutboxBridgeTests.cs` (EVENTING-3).
- [ ] **Architecture**: `EventingArchitectureTests.cs` (EVENTING-4).
- [ ] **Integration**: `EventingBridgeIntegrationTests.cs` (EVENTING-3/4).
- [ ] **Functional**: `EventingBridgeFunctionalTests.cs` in `Identity.Tests` (EVENTING-5).

## 2. Implementation (Green)
- [ ] Implement `DomainEventToOutboxBridge<TEvent>` (EVENTING-3 - Option A).
- [ ] Register bridge in DI (EVENTING-4).
- [ ] Implement `UserCreatedIntegrationEvent` and translator (EVENTING-5 - Option B demo).
- [ ] Wire other Identity domain events to `IIntegrationEvent` (EVENTING-6 - Option A).
- [ ] Enable multitenancy for `FshRole` (IDENTITY-3).

## 3. Verification & Polish
- [ ] Ensure all local tests pass (`dotnet test`).
- [ ] Ensure 0 build warnings.
- [ ] Architecture verification.

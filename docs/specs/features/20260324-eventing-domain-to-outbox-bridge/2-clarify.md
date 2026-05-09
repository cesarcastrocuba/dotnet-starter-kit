# Clarifications: feat/eventing-domain-to-outbox-bridge — EVENTING-3/4/5

## Unresolved Questions

1. **[Registration Scope]**: Should the bridge be registered in `EventingModule.cs` or `EventingExtensions.cs`?
   - *Decision*: We will register it in `EventingExtensions.cs` as part of the `AddEventing` flow for consistency.
2. **[Dual Interface]**: Are most domain events expected to implement `IIntegrationEvent`?
   - *Decision*: Only events that need to be consumed externally or across modules should implement both. This bridge provides an opt-in mechanism by interface inheritance.

## Decisions Made

- **Primary Pattern**: Implement **Option A** (Generic Bridge) for standard events that implement both `IDomainEvent` and `IIntegrationEvent`.
- **Demo Pattern**: Implement **Option B** for `UserRegisteredEvent` -> `UserCreatedIntegrationEvent` to demonstrate explicit mapping when schemas differ.
- **Multitenancy**: Include `FshRole` multitenancy (IDENTITY-3) to ensure full isolation in the Identity module.

# Technical Plan: feat/eventing-domain-to-outbox-bridge — EVENTING-3/4/5

## Architecture & Design

The `DomainEventToOutboxBridge<TEvent>` will act as a standard `INotificationHandler` for any event `TEvent` that implements both `IDomainEvent` and `IIntegrationEvent`. When such an event is published via `IPublisher`, Mediator will invoke the bridge, which will then use `IOutboxStore.SaveAsync` to persist the event.

## Proposed Changes

### BuildingBlocks.Eventing (Option A)
#### [NEW] [DomainEventToOutboxBridge.cs](file:///c:/github/dotnet-starter-kit/src/BuildingBlocks/Eventing/Bridge/DomainEventToOutboxBridge.cs)
- Implement the open-generic bridge.
#### [MODIFY] [EventingExtensions.cs](file:///c:/github/dotnet-starter-kit/src/BuildingBlocks/Eventing/EventingExtensions.cs)
- Register the bridge in the DI container.

### Modules.Identity (Option B & Wiring)
#### [NEW] [UserCreatedIntegrationEvent.cs](file:///c:/github/dotnet-starter-kit/src/Modules/Identity/Modules.Identity/Events/UserCreatedIntegrationEvent.cs)
- Specific integration event for demonstration of explicit mapping (Option B).
#### [NEW] [UserRegisteredDomainEventHandler.cs]
- Handler that translates `UserRegisteredEvent` to `UserCreatedIntegrationEvent` (Option B).
#### [MODIFY] [Identity Events]
- Update other events to implement `IIntegrationEvent` (Option A wiring).

## Testing Strategy

- **Unit Tests**: `Generic.Tests/Eventing/DomainEventToOutboxBridgeTests.cs` using NSubstitute to verify bridge logic and DI registration.
- **Integration Tests**: `Generic.Tests/Eventing/EventingBridgeIntegrationTests.cs` using a real `OutboxStore` and database via Testcontainers to verify persistence.
- **Functional Tests**: `Identity.Tests/Functional/UserRegistrationFlowTests.cs` (or similar) to verify the end-to-end flow: Command triggers Domain Event -> Bridge saves to Outbox -> Dispatcher publishes -> Integration Handler receives.
- **Architecture Tests**: `Architecture.Tests/EventingArchitectureTests.cs` to verify dependencies and bridge registration rules.

# Specification: feat/eventing-domain-to-outbox-bridge — Automatic Bridge (#8)

## 1. Description

Domain Events published via `IPublisher` (Mediator) are currently in-process only. This specification introduces an automatic mechanism to bridge domain events into the Outbox for cross-module async delivery and eventual external publishing (e.g., RabbitMQ).

## 2. Requirements & User Stories

- **EVENTING-3 (Option A - Generic Bridge)**: Create `DomainEventToOutboxBridge<T>` in `BuildingBlocks/Eventing/Bridge/` to automatically route dual-interface events.
- **EVENTING-4 (Registration)**: Register the bridge as an open-generic `INotificationHandler<>` in `EventingExtensions.cs`.
- **EVENTING-5 (Option B - Explicit Mapping Demo)**: Create `UserCreatedIntegrationEvent` and a specific handler in `Modules.Identity` to demonstrate the explicit mapping pattern.
- **EVENTING-6 (Domain Event Wiring)**: Audit and update Identity events to implement `IIntegrationEvent` where dual-purpose is appropriate (Option A).
- **IDENTITY-3 (Multitenancy Check)**: Enable multitenancy for `FshRole` as requested.

- [ ] Acceptance Criteria
- [ ] `DomainEventToOutboxBridge<T>` exists in `BuildingBlocks/Eventing/Bridge/`.
- [ ] Bridge is registered as open-generic in `EventingExtensions.cs`.
- [ ] Publishing a dual-interface domain event results in an outbox record.
- [ ] `UserCreatedIntegrationEvent` is correctly implemented in Identity module.
- [ ] **Unit Tests**: Coverage for bridge logic and DI registration.
- [ ] **Integration Tests**: Verify database persistence in outbox using Testcontainers.
- [ ] **Functional Tests**: Verify end-to-end flow from API/Command -> Domain Event -> Outbox -> Dispatcher -> Subscriber.
- [ ] **Architecture Tests**: Ensure no violations of layering or module boundaries.
- [ ] 0 build warnings, 0 errors.

# Clarifications: Tenant Pattern Unification (#11)

## Unresolved Questions
(All currently resolved through discussion)

## Decisions Made

### 1. Architecture Testing Location
- **Decision**: `TenancyIsolationTests.cs` will be placed directly in `src/Tests/Architecture.Tests/`.
- **Reasoning**: Follow user preference for a flatter folder structure in architecture tests.

### 2. Integration Testing Centralization
- **Decision**: All multi-tenancy integration tests (including those for BuildingBlocks like Eventing) will be located in the `src/Tests/Integration.Tests/` project.
- **Paths**: 
  - `src/Tests/Integration.Tests/Tenancy/TenantIsolationIntegrationTests.cs`
  - `src/Tests/Integration.Tests/Eventing/EventingIsolationIntegrationTests.cs`
- **Reasoning**: Keep cross-cutting integration concerns in a centralized project using the Testcontainers infrastructure.

### 3. Functional Testing Folder
- **Decision**: A new `Multitenancy` folder will be created in `src/Tests/Functional.Tests/`.
- **Reasoning**: Current functional tests only cover Identity; adding Multitenancy improves organization and coverage.

### 4. Unit Testing Strategy
- **Decision**: `src/Tests/Multitenancy.Tests/Domain/` tests will be updated to ensure logical coverage of the entities being mapped as `.IsMultiTenant()`.
- **Reasoning**: Ensure domain logic is sound independently of the persistence layer.

### 5. Strategy Refinement
- **Decision**: Removed references to `Spec.Tests` and `Generic.Tests` from the main testing commands/strategy to focus on the core test projects.
- **Reasoning**: Streamline the verification process and align with the actual project structure.

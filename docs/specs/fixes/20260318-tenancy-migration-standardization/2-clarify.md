# Clarifications: Tenancy Migration & Standardization

## Resolved Decisions

1. **Identity Module Migration**:
   - **Decision**: `TenantId` is standardized at **64** characters everywhere for consistency with the Multitenancy module.
   - **Implementation**: Added explicit `HasMaxLength(64)` and `.IsMultiTenant()` to `UserSession`, `PasswordHistory`, `GroupRole`, and `UserGroup`.

2. **DateTimeOffset Standardization**:
   - **Decision**: Perform a **project-wide** standardization. Every `DateTime` used for timestamps is now `DateTimeOffset`, and the suffix `OnUtc` is mandatory.
   - **Rationale**: Better PostgreSQL compatibility and multi-region accuracy.
   - **Naming**: Properties like `CreatedUtc`, `StartedUtc`, `ValidUpto`, `ReceivedAtUtc` have all been renamed to `CreatedOnUtc`, `StartedOnUtc`, `ValidUptoOnUtc`, `ReceivedOnUtc`, etc.

3. **TenantId Consistency**:
   - **Decision**: `TenantId` is required where isolation is mandatory. In `InboxMessage` and `AuditRecord`, it remains mandatory to ensure data belongs to a specific tenant boundary.
   - **Naming**: Unified naming to `TenantId`.

4. **Migration Contexts**:
   - **Decision**: Unified migrations within the `src/Playground/Migrations.PostgreSQL` project, organized into subfolders by module:
     - `Identity/`
     - `Auditing/` (consolidated from legacy `Audit/`)
     - `MultiTenancy/`

5. **Test Precision**:
   - **Decision**: When asserting `DateTimeOffset` equality with `ShouldBe`, use a small tolerance or ensure UTC consistency (using `.ToUniversalTime()`) in constructors (e.g., `AuditEnvelope`).
   - **NSubstitute**: Resolved `ValueTask` return type issues in `GenerateTokenCommandHandlerTests` by correctly mocking async returns.

## Final Decisions (Finalized)
- **DateTime Format**: Use `DateTimeOffset` + `OnUtc` naming everywhere.
- **Isolation**: Explicit `IsMultiTenant()` and `HasMaxLength(64)` for all tenant-aware entities.
- **Metadata**: Remove `HasColumnName` aliases to keep the schema clean and mapped directly to domain properties.

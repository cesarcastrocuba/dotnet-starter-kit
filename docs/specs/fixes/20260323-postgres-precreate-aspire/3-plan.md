# Technical Plan: fix/postgres-precreate-aspire

## Architecture & Design

This fix lives entirely in `BuildingBlocks/Persistence` — the infrastructure layer responsible for
database configuration and EF Core lifecycle. The new `DatabasePrecreatorHostedService` follows the
**exact same pattern** as the existing `DatabaseOptionsStartupLogger` IHostedService:
- Injected: `IOptions<DatabaseOptions>` + `ILogger<T>`
- Runs in `StartAsync()`, is a no-op in `StopAsync()`
- Registered in `AddHeroDatabaseOptions()` so it runs before any module `IDbInitializer`

**Registration order guarantee**: Since `IHostedService` implementations run in DI registration order,
`DatabasePrecreatorHostedService` must be registered **before** any module registers its
`IDbInitializer` hosted service. We achieve this by adding the registration inside
`AddHeroDatabaseOptions()`, which is always called before module DI setup.

**No new NuGet packages**: `Persistence.csproj` already references:
- `Npgsql.EntityFrameworkCore.PostgreSQL` → provides `NpgsqlConnectionStringBuilder` + `NpgsqlConnection`
- `Microsoft.EntityFrameworkCore.SqlServer` → provides `SqlConnectionStringBuilder` + `SqlConnection`
- `Microsoft.Extensions.Hosting` → provides `IHostedService`

**AppHost.cs**: No changes needed. `.AddDatabase("fsh")` is already present (Aspire metadata only).

---

## Proposed Changes (File Level)

### BuildingBlocks/Persistence

#### [NEW] `DatabasePrecreatorHostedService.cs`
Path: `src/BuildingBlocks/Persistence/DatabasePrecreatorHostedService.cs`
- Implements `IHostedService`
- Reads `DatabaseOptions.Provider` (case-insensitive) to choose PostgreSQL or MSSQL path
- **PostgreSQL path**: uses `NpgsqlConnectionStringBuilder` to extract `Database` name, connects to
  `postgres` system DB, runs `SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname = '{db}')`,
  creates if not found
- **MSSQL path**: uses `SqlConnectionStringBuilder`, connects to `master`, uses
  `IF NOT EXISTS ... CREATE DATABASE` DDL
- **Unknown provider**: logs a warning and skips gracefully (defensive programming)
- All code paths emit structured `ILogger` events
- `StopAsync` → `Task.CompletedTask` (no cleanup needed)

#### [MODIFY] `PersistenceExtensions.cs`
Path: `src/BuildingBlocks/Persistence/PersistenceExtensions.cs`
- In `AddHeroDatabaseOptions()`: add `services.AddHostedService<DatabasePrecreatorHostedService>();`
  immediately **after** `ValidateOnStart()` and **before** `services.AddHostedService<DatabaseOptionsStartupLogger>()`
  (or just before `AddPersistenceServices()`).

---

### Tests

#### [NEW] `DatabasePrecreatorHostedServiceTests.cs`
Path: `src/Tests/Generic.Tests/Infrastructure/DatabasePrecreatorHostedServiceTests.cs`

Unit tests using `NSubstitute` + `Shouldly` + `xunit`. Since the service makes real DB connections,
we test it via **constructor and logic isolation**: mock `IOptions<DatabaseOptions>` and `ILogger`,
test that:
1. **PostgreSQL provider**: `StartAsync` calls the PostgreSQL branch (verified via mock logger invocation
   and no exception thrown for well-formed connection strings where we mock the DB)
2. **MSSQL provider**: Same pattern for MSSQL
3. **Unknown provider**: `StartAsync` completes without exception and logs a warning
4. **StopAsync**: Always returns `Task.CompletedTask` regardless of input
5. **Constructor null guards**: `ArgumentNullException` thrown for null `IOptions<DatabaseOptions>`

> Note: The PostgreSQL/MSSQL "creates database" paths require a real DB connection, so they are
> **not** unit-testable in isolation. The positive "creates DB" scenario is covered by the Integration
> Test infrastructure (existing `CustomWebApplicationFactory` spins up real Testcontainers).

#### [NEW] Architecture test assertion in `BuildingBlocksIndependenceTests.cs`
Actually — no new test class needed. The existing test `BuildingBlocks_Should_Follow_Layered_Dependencies`
already validates `Persistence depends on [Core, Shared]` and will pass since we add no new project
references. We add one targeted assertion in `PersistenceHostedServicesTests.cs` (new file in
`Architecture.Tests`) to verify the `DatabasePrecreatorHostedService` is in the correct namespace and
assembly.

#### [NEW] `PersistenceHostedServicesTests.cs`
Path: `src/Tests/Architecture.Tests/PersistenceHostedServicesTests.cs`
- Uses `NetArchTest.Rules` (already in `Architecture.Tests.csproj`)
- Verifies `DatabasePrecreatorHostedService` resides in assembly `FSH.Framework.Persistence`
- Verifies it implements `IHostedService`
- Verifies it is `sealed`

---

## Testing Strategy

### Unit Tests (Generic.Tests)
New file: `src/Tests/Generic.Tests/Infrastructure/DatabasePrecreatorHostedServiceTests.cs`

```bash
dotnet test src/Tests/Generic.Tests --filter "DatabasePrecreator"
```

Covers:
- Constructor null-guard for `IOptions<DatabaseOptions>`
- StopAsync is a no-op
- Unknown provider → logs warning, no exception
- Service can be instantiated with mocked dependencies and valid PostgreSQL-format connection string

### Architecture Tests (Architecture.Tests)
New file: `src/Tests/Architecture.Tests/PersistenceHostedServicesTests.cs`

```bash
dotnet test src/Tests/Architecture.Tests --filter "PersistenceHostedServices"
```

### Full Test Suite
```bash
dotnet build src/FSH.Framework.slnx  # Must be 0 warnings, 0 errors
dotnet test src/FSH.Framework.slnx   # All tests must pass
```

### Manual Verification (Aspire first-run)
```bash
# Simulate fresh environment
docker volume rm fsh-postgres-data fsh-redis-data

# Run the app
dotnet run --project src/Playground/FSH.Playground.AppHost

# Expected in logs:
# [INF] DatabasePrecreatorHostedService: Creating PostgreSQL database 'fsh'
# (no NpgsqlException: FATAL: database "fsh" does not exist)
```

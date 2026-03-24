# Implementation: fix/postgres-precreate-aspire

Successfully implemented `DatabasePrecreatorHostedService` to resolve the first-run
`"database does not exist"` crash when using Aspire with empty Docker volumes. No deviations from the plan.

## 1. Technical Implementation Summary

### New File: `src/BuildingBlocks/Persistence/DatabasePrecreatorHostedService.cs`
- Implements `IHostedService` (sealed, follows `DatabaseOptionsStartupLogger` pattern)
- Reads `DatabaseOptions.Provider` (case-insensitive) to branch on provider type
- **PostgreSQL path**: uses `NpgsqlConnectionStringBuilder` to connect to `postgres` system DB,
  checks `pg_database` catalog, creates DB only if not found
- **MSSQL path**: uses `SqlConnectionStringBuilder` to connect to `master`, uses
  `IF NOT EXISTS ... CREATE DATABASE` idempotent DDL
- **Unknown provider**: `IsEnabled(LogLevel.Warning)` guarded warning log, no exception
- All `Log*` calls guarded with `IsEnabled()` to satisfy CA1873 analyzer rule
- `CA2100` pragmas on DDL string construction with inline comments explaining why it's safe
  (value comes from parsed, validated configuration — not user input)
- `await using` on all `IDbCommand` instances to satisfy CA2000

### Modified File: `src/BuildingBlocks/Persistence/PersistenceExtensions.cs`
- Added `services.AddHostedService<DatabasePrecreatorHostedService>()` in `AddHeroDatabaseOptions()`
  **before** `DatabaseOptionsStartupLogger` and all module `IDbInitializer` registrations
- Inline comment explains the registration order guarantee

### Modified File: `src/Tests/Generic.Tests/Generic.Tests.csproj`
- Added `<ProjectReference>` to `Persistence.csproj` so unit tests can reference
  `DatabasePrecreatorHostedService` and `DatabaseOptions`

### New File: `src/Tests/Generic.Tests/Infrastructure/DatabasePrecreatorHostedServiceTests.cs`
- 9 unit tests (NSubstitute + Shouldly + xunit):
  - Constructor null guard for `IOptions<DatabaseOptions>`
  - Constructor success with valid mocked dependencies
  - `StopAsync` returns completed task
  - Unknown providers (4 theory cases) — `StartAsync` completes without exception
  - Unknown provider triggers `IsEnabled(LogLevel.Warning)` check
  - Service implements `IHostedService`

### New File: `src/Tests/Architecture.Tests/PersistenceHostedServicesTests.cs`
- 5 architecture tests (NetArchTest.Rules + Shouldly):
  - Service is in `FSH.Framework.Persistence` assembly
  - Service is sealed
  - Service implements `IHostedService`
  - Service is in correct namespace (`FSH.Framework.Persistence`)
  - Persistence assembly has no module dependencies

## 2. Verification Report

### Automated Tests
| Suite | Passed | Failed |
|---|---|---|
| Generic.Tests | 60 | 0 |
| Architecture.Tests | 56 | 0 |
| Identity.Tests | 221 | 0 |
| Auditing.Tests | 61 | 0 |
| Multitenancy.Tests | 97 | 0 |
| Integration.Tests | 4 | 0 |
| Functional.Tests | 5 | 0 |
| Spec.Tests | 1 | 0 |
| **TOTAL** | **505** | **0** |

### Build Verification
```
dotnet build src/FSH.Framework.slnx
Build succeeded.
  0 Warning(s)
  0 Error(s)
```

## 3. Final Artifacts

- **Branch**: `fix/postgres-precreate-aspire`
- **Commit**: `f10ea3cb` — "fix: pre-create database on first Aspire startup (issue #16)"
- **Spec folder**: `docs/specs/fixes/20260323-postgres-precreate-aspire/`
- **Closes**: GitHub Issue #16

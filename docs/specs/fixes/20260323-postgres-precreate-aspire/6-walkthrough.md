# Walkthrough: fix/postgres-precreate-aspire

## Overview

We found that on first Aspire startup with empty Docker volumes, the PostgreSQL container
becomes healthy before the `"fsh"` database is created. EF Core's `MigrateAsync()` then
crashed with `FATAL: database "fsh" does not exist`. We fixed it by introducing a new
`DatabasePrecreatorHostedService` that runs first and creates the database if missing.

---

## Architecture Guard Report

### BuildingBlocks
✅ No modifications to existing BuildingBlocks APIs — added one new file only

### Architecture Tests
✅ All 56 Architecture.Tests passed (including 5 new `PersistenceHostedServicesTests`)

### Build Warnings
✅ 0 warnings — CA2100 suppressed with explanatory comments, CA2000 fixed with `await using`,
CA1873 fixed with `IsEnabled()` guards

### Module Boundaries
✅ Clean — `DatabasePrecreatorHostedService` has zero module dependencies

### Mediator Usage
✅ Not applicable — this is an infrastructure service, no CQRS involvement

### Validators
✅ Not applicable — no new commands

### Authorization
✅ Not applicable — no new endpoints

**Overall: ✅ PASS**

---

## Code Review Summary

### ✅ Passed
- `DatabasePrecreatorHostedService` is `sealed`, matches `DatabaseOptionsStartupLogger` pattern
- XML doc comments on all public members
- Structured logging with named placeholders (`'{DbName}'`, `'{Provider}'`)
- `ArgumentNullException.ThrowIfNull(options)` null guard in constructor
- `await using` on all disposable DB objects
- `ConfigureAwait(false)` on all awaits in library code
- Registration order in `AddHeroDatabaseOptions()` clearly documented with inline comment
- No cross-module dependencies

### ⚠️ Notes
- **CA2100 suppressions**: DDL strings are constructed from connection string configuration
  (validated at startup via `ValidateOnStart()`) — not from user input. This is intentional
  and safe. PostgreSQL/MSSQL DDL cannot use parameterized queries for `CREATE DATABASE`.

---

## Key Learnings & Technical Debt

- **IHostedService registration order matters**: The fix works because `AddHeroDatabaseOptions()`
  is always called before module DI setup in the startup pipeline.
- **Idempotent by design**: The service can safely run on every startup without side effects.
- **MSSQL path untestable without containers**: The `IF NOT EXISTS` MSSQL path is implemented
  but cannot be exercised in unit tests. Full verification requires a SQL Server container.

---

## Deployment Notes

- No database migrations required
- No `appsettings.json` changes required
- No `AppHost.cs` changes required
- Merge branch `fix/postgres-precreate-aspire` → `develop` via PR
- After merge, close GitHub Issue #16

# Specification: fix/postgres-precreate-aspire — Database does not exist on first Aspire startup

## 1. Description

On the **very first run** with Aspire (empty Docker volumes), the Aspire orchestrator waits for the
PostgreSQL container to become healthy via `.WaitFor(postgres)`, but it does **not** pre-create the
application database `"fsh"`. When the application boots, `AppDbInitializer.MigrateAsync()` calls
EF Core `MigrateAsync()` which requires the target database to already exist — resulting in:

```
Unhandled exception. Npgsql.NpgsqlException: FATAL: database "fsh" does not exist
  at FSH.Framework.Persistence.Initializers.AppDbInitializer.MigrateAsync()
```

This is the **#1 first-run blocker** for any developer cloning the repo and running Aspire.

## 2. Requirements & User Stories

- **REQ-1**: On first Aspire startup with empty Docker volumes, the application must start without
  any `"database does not exist"` error.
- **REQ-2**: If the target database already exists (subsequent runs), the service must be a no-op
  (idempotent). No errors, no warnings, no duplicate-create attempts.
- **REQ-3**: The solution must support **PostgreSQL** (primary path) and **MSSQL** (secondary path),
  reading the provider from `DatabaseOptions`.
- **REQ-4**: The database pre-creation must happen **before** any EF Core migration hosted services run,
  so registration order in DI is critical.
- **REQ-5**: The service must emit structured `ILogger` log messages so operators can observe the
  pre-creation event.
- **REQ-6**: Zero new build warnings. 0 errors.

## 3. Acceptance Criteria

- [ ] `DatabasePrecreatorHostedService` created in `src/BuildingBlocks/Persistence/`
- [ ] Service registered via `AddHeroDatabaseOptions()` in `PersistenceExtensions.cs`, before any
  `IDbInitializer` hosted services
- [ ] On first Aspire run (empty volumes), app starts cleanly — no `"database does not exist"` error
- [ ] On subsequent runs, service runs silently (idempotent)
- [ ] Both PostgreSQL and MSSQL code paths compile and behave correctly
- [ ] Unit tests cover: PostgreSQL path (creates), PostgreSQL path (already exists / no-op), MSSQL path,
  unknown provider (graceful skip)
- [ ] Architecture tests verify the new service is correctly injectable from `BuildingBlocks/Persistence`
- [ ] `dotnet build src/FSH.Framework.slnx` reports 0 warnings, 0 errors
- [ ] `dotnet test src/FSH.Framework.slnx` — all tests pass

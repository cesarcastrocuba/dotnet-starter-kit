# Clarifications: fix/postgres-precreate-aspire

## Decisions Made

All answers derived from direct codebase inspection — no open questions remain.

### 1. BuildingBlocks modification — requires approval?
The issue explicitly places the new file in `src/BuildingBlocks/Persistence/DatabasePrecreatorHostedService.cs`.
This is a **net-new file** in a BuildingBlock, not a modification of an existing API surface.
The `Persistence.csproj` already references `Npgsql.EntityFrameworkCore.PostgreSQL` and
`Microsoft.EntityFrameworkCore.SqlServer`, so **no new NuGet packages are required**.

**Decision**: Proceed. The issue author explicitly calls for this location.

### 2. Where is `DatabaseOptions` defined?
`src/BuildingBlocks/Shared/Persistence/DatabaseOptions.cs` — namespace `FSH.Framework.Shared.Persistence`.
The `DatabaseOptions.Provider` defaults to `"POSTGRESQL"` and also supports `"MSSQL"`.

**Decision**: Read `Provider` string (case-insensitive comparison) to choose the code path.

### 3. How should PostgreSQL connection be established for pre-creation?
The issue's skeleton uses `NpgsqlConnectionStringBuilder` to extract the `Database` property,
then connects to the system `postgres` database to issue `CREATE DATABASE`.

For **MSSQL**, connect to `master` and use `IF NOT EXISTS` DDL.

**Decision**: Follow the issue's code skeleton exactly — it is the intended implementation.

### 4. Registration order: where exactly in `PersistenceExtensions.cs`?
`AddHeroDatabaseOptions()` currently registers `DatabaseOptionsStartupLogger` and calls
`AddPersistenceServices()`. The `DatabasePrecreatorHostedService` must be registered **before** any
`IDbInitializer`-based hosted services. The `DatabaseOptionsStartupLogger` is purely a _logger_ so
order relative to it is irrelevant.

**Decision**: Register `DatabasePrecreatorHostedService` inside `AddHeroDatabaseOptions()`,
immediately after options binding and `ValidateOnStart()`, so it is guaranteed to run before any module
registers its own `IDbInitializer` hosted service.

### 5. SQL injection risk in `CREATE DATABASE`?
The database name comes from the parsed connection string (`NpgsqlConnectionStringBuilder.Database`),
not from user input at request time. The value is validated at startup by `DatabaseOptions` validation.
Using parameterized queries for DDL statements (`CREATE DATABASE`) is not supported by PostgreSQL or
SQL Server.

**Decision**: Use string interpolation with the parsed DB name (same pattern as issue skeleton).
Annotate with a code comment to explain this is intentional and why it is safe.

### 6. Which test project for unit tests?
`Generic.Tests` — it references `BuildingBlocks` directly, uses `NSubstitute`, and already exercises
infrastructure-level services (storage, eventing, exceptions). It does not require Testcontainers.

**Decision**: Add `DatabasePrecreatorHostedServiceTests.cs` to `Generic.Tests/Infrastructure/`.

### 7. AppHost.cs changes?
The `AppHost.cs` already has `.AddDatabase("fsh")` and `.WaitFor(postgres)`. No change needed —
the issue confirms this is `PERSISTENCE-3 (optional)` and it is already correctly set.

**Decision**: No changes to `AppHost.cs`.

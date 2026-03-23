# Implementation Tasks: fix/postgres-precreate-aspire

## 1. Test Setup (Red)
- [ ] Write unit tests `Generic.Tests/Infrastructure/DatabasePrecreatorHostedServiceTests.cs`
  - [ ] Constructor: null `IOptions<DatabaseOptions>` throws `ArgumentNullException`
  - [ ] `StopAsync`: always returns `Task.CompletedTask`
  - [ ] Unknown provider: `StartAsync` completes without exception, no DB calls
  - [ ] Service can be instantiated with valid mocked dependencies (PostgreSQL connection string format)
- [ ] Write architecture tests `Architecture.Tests/PersistenceHostedServicesTests.cs`
  - [ ] `DatabasePrecreatorHostedService` is in `FSH.Framework.Persistence` assembly
  - [ ] `DatabasePrecreatorHostedService` implements `IHostedService`
  - [ ] `DatabasePrecreatorHostedService` is sealed

## 2. Implementation (Green)
- [ ] Create `src/BuildingBlocks/Persistence/DatabasePrecreatorHostedService.cs`
  - [ ] PostgreSQL path: connect to `postgres` system DB, check existence, `CREATE DATABASE`
  - [ ] MSSQL path: connect to `master`, `IF NOT EXISTS CREATE DATABASE`
  - [ ] Unknown provider: log warning, return gracefully
  - [ ] XML doc comments on all public members
- [ ] Modify `src/BuildingBlocks/Persistence/PersistenceExtensions.cs`
  - [ ] Register `DatabasePrecreatorHostedService` in `AddHeroDatabaseOptions()` before other hosted services

## 3. Verification & Polish
- [ ] `dotnet build src/FSH.Framework.slnx` → 0 warnings, 0 errors
- [ ] `dotnet test src/FSH.Framework.slnx` → all tests pass
- [ ] Architecture guard check (BuildingBlocks not depending on Modules)
- [ ] Code review check (no MediatR, correct patterns)
- [ ] Create `5-implement.md` report
- [ ] Create `6-walkthrough.md`

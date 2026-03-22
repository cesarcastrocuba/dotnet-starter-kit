# Walkthrough: Tenant Pattern Unification (#11)

We found that multiple entities holding a `TenantId` property within the Multitenancy module and Eventing building blocks were missing their `.IsMultiTenant()` configurations, opening a risk of cross-tenant data leakage if queried without strict `Where` clauses. We fixed this by asserting `.IsMultiTenant()` globally on these entities, but this introduced unexpected side effects. We subsequently verified the fixes by aligning EF Core migrations, bypassing the newly introduced query filters in out-of-band background services, and ensuring the tests executed with `0 errors and 0 warnings`.

## 1. Visual Evidence / Logs

```bash
Passed!  - Failed:     0, Passed:    61, Skipped:     0, Total:    61, Duration: 521 ms - Auditing.Tests.dll
Passed!  - Failed:     0, Passed:   221, Skipped:     0, Total:   221, Duration: 359 ms - Identity.Tests.dll
Passed!  - Failed:     0, Passed:    97, Skipped:     0, Total:    97, Duration: 1 s - Multitenancy.Tests.dll
Passed!  - Failed:     0, Passed:    51, Skipped:     0, Total:    51, Duration: 1 s - Generic.Tests.dll
Passed!  - Failed:     0, Passed:    50, Skipped:     0, Total:    50, Duration: 1 s - Architecture.Tests.dll
Passed!  - Failed:     0, Passed:     3, Skipped:     0, Total:     3, Duration: 19 s - Integration.Tests.dll
Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5, Duration: 12 s - Functional.Tests.dll
Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: 5 ms - Spec.Tests.dll
```

A clean rebuild confirmed standard compliance:
`Build succeeded. 0 Warning(s), 0 Error(s)`

## 2. Key Learnings & Technical Debt

- **Finbuckle Side Effects**: Applying `.IsMultiTenant()` enforces Global Query Filters. This means that background services (which don't have an HTTP context or a Tenant Header) and cross-tenant API requests (like provisioning) will suddenly fail to find records. Use `.IgnoreQueryFilters()` surgically for repositories or DbSets responding to these edge cases.
- **Log Noise Handling**: The `GlobalExceptionHandler` was throwing heavy red `[ERR]` console records for common validation blocks (`400`) and missing resources (`404`). Routing these to `[WRN]` drastically cleans up telemetry tracking in production.
- **Test Integrity**: Testcontainers are invaluable here. The functional tests exposed the true database schema mapping gap (missing `TenantId` column in Postgres) rather than skipping it as an `InMemory` provider might.

## 3. Deployment Notes

- EF Core Migrations have been added to the `TenantDbContext` and `IdentityDbContext` to reflect the newly enforced multitenancy structural mapping. Ensure `RunTenantMigrationsOnStartup` is active, or apply the generated SQL migration scripts to staging/production prior to launching the updated artifact in `develop`.

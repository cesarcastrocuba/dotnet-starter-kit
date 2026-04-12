using FSH.Framework.Shared.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FSH.Framework.Persistence;

/// <summary>
/// Hosted service that runs at application startup — before EF Core migrations — to ensure
/// the target database exists. On first Aspire run with empty Docker volumes, the PostgreSQL
/// container is healthy but the application database has not been created yet.
/// This service creates the database if it does not exist (idempotent: no-op if it already exists).
/// Supports <c>POSTGRESQL</c> and <c>MSSQL</c> providers as configured in <see cref="DatabaseOptions"/>.
/// </summary>
public sealed class DatabasePrecreatorHostedService : IHostedService
{
    private readonly DatabaseOptions _options;
    private readonly ILogger<DatabasePrecreatorHostedService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasePrecreatorHostedService"/> class.
    /// </summary>
    /// <param name="options">Database configuration options containing provider and connection string.</param>
    /// <param name="logger">Logger for emitting structured startup events.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public DatabasePrecreatorHostedService(
        IOptions<DatabaseOptions> options,
        ILogger<DatabasePrecreatorHostedService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Ensures the configured database exists before EF Core migrations run.
    /// Connects to the system database (<c>postgres</c> for PostgreSQL, <c>master</c> for MSSQL)
    /// and issues a <c>CREATE DATABASE</c> command if needed.
    /// </summary>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_options.Provider.Equals(DbProviders.PostgreSQL, StringComparison.OrdinalIgnoreCase))
        {
            await EnsurePostgresDatabaseAsync(cancellationToken).ConfigureAwait(false);
        }
        else if (_options.Provider.Equals(DbProviders.MSSQL, StringComparison.OrdinalIgnoreCase))
        {
            await EnsureMssqlDatabaseAsync(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(
                    "DatabasePrecreatorHostedService: Unknown provider '{Provider}'. " +
                    "Database pre-creation skipped. Supported providers: {Supported}.",
                    _options.Provider,
                    string.Join(", ", DbProviders.PostgreSQL, DbProviders.MSSQL));
            }
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    // ---------------------------------------------------------------------------
    // PostgreSQL
    // ---------------------------------------------------------------------------

    private async Task EnsurePostgresDatabaseAsync(CancellationToken ct)
    {
        var builder = new NpgsqlConnectionStringBuilder(_options.ConnectionString);

        // Extract the target database name; fall back to "fsh" if not specified.
        var dbName = builder.Database ?? "fsh";

        // Connect to the PostgreSQL system database to run administrative DDL.
        // Connecting to the target DB directly would fail if it doesn't exist yet.
        builder.Database = "postgres";

        await using var connection = new NpgsqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);

        // CA2100 suppressed: dbName is parsed from the application connection string via
        // NpgsqlConnectionStringBuilder — it originates from validated configuration, not
        // from untrusted user input. CREATE DATABASE DDL cannot use parameterized queries
        // in PostgreSQL.
#pragma warning disable CA2100
        await using var checkCmd = new NpgsqlCommand(
            $"SELECT EXISTS(SELECT 1 FROM pg_database WHERE datname = '{dbName}')",
            connection);
#pragma warning restore CA2100

        var exists = (bool)(await checkCmd.ExecuteScalarAsync(ct).ConfigureAwait(false))!;

        if (!exists)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    "DatabasePrecreatorHostedService: PostgreSQL database '{DbName}' does not exist. Creating...",
                    dbName);
            }

            // CREATE DATABASE cannot be run inside a transaction in PostgreSQL.
            // Quoting the name handles names with special characters.
#pragma warning disable CA2100
            await using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\"", connection);
#pragma warning restore CA2100
            await createCmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(
                    "DatabasePrecreatorHostedService: PostgreSQL database '{DbName}' created successfully.",
                    dbName);
            }
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "DatabasePrecreatorHostedService: PostgreSQL database '{DbName}' already exists. Skipping.",
                    dbName);
            }
        }
    }

    // ---------------------------------------------------------------------------
    // MSSQL
    // ---------------------------------------------------------------------------

    private async Task EnsureMssqlDatabaseAsync(CancellationToken ct)
    {
        var builder = new SqlConnectionStringBuilder(_options.ConnectionString);

        // Extract the target database name; fall back to "fsh" if not specified.
        var dbName = string.IsNullOrWhiteSpace(builder.InitialCatalog) ? "fsh" : builder.InitialCatalog;

        // Connect to master to run administrative DDL.
        builder.InitialCatalog = "master";

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);

        // CA2100 suppressed: dbName is parsed from the application connection string via
        // SqlConnectionStringBuilder — it originates from validated configuration, not
        // from untrusted user input. CREATE DATABASE DDL cannot use parameterized queries.
#pragma warning disable CA2100
        await using var cmd = new SqlCommand(
            $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}') " +
            $"CREATE DATABASE [{dbName}]",
            connection);
#pragma warning restore CA2100

        await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "DatabasePrecreatorHostedService: Ensured MSSQL database '{DbName}' exists.",
                dbName);
        }
    }
}

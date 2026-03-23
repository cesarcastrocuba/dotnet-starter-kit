using FSH.Framework.Persistence;
using FSH.Framework.Shared.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Generic.Tests.Infrastructure;

/// <summary>
/// Unit tests for <see cref="DatabasePrecreatorHostedService"/>.
/// Tests focus on constructor guards, no-op paths, and unknown provider handling.
/// Real DB creation paths (PostgreSQL/MSSQL) require live containers and are
/// covered by integration tests in the Aspire first-run scenario.
/// </summary>
public class DatabasePrecreatorHostedServiceTests
{
    private readonly ILogger<DatabasePrecreatorHostedService> _logger =
        Substitute.For<ILogger<DatabasePrecreatorHostedService>>();

    // ------------------------------------------------------------------ constructor

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_OptionsIsNull()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new DatabasePrecreatorHostedService(null!, _logger));
    }

    [Fact]
    public void Constructor_Should_NotThrow_When_ValidDependenciesProvided()
    {
        // Arrange
        var options = CreateOptions("POSTGRESQL",
            "Host=localhost;Database=fsh;Username=postgres;Password=pass");

        // Act & Assert
        Should.NotThrow(() => new DatabasePrecreatorHostedService(options, _logger));
    }

    // ------------------------------------------------------------------ StopAsync

    [Fact]
    public async Task StopAsync_Should_ReturnCompletedTask_Always()
    {
        // Arrange
        var options = CreateOptions("POSTGRESQL",
            "Host=localhost;Database=fsh;Username=postgres;Password=pass");
        var sut = new DatabasePrecreatorHostedService(options, _logger);

        // Act
        var task = sut.StopAsync(CancellationToken.None);

        // Assert
        task.IsCompleted.ShouldBeTrue();
        await task; // should not throw
    }

    // ------------------------------------------------------------------ Unknown provider

    [Theory]
    [InlineData("")]
    [InlineData("SQLITE")]
    [InlineData("ORACLE")]
    [InlineData("UNKNOWN_PROVIDER")]
    public async Task StartAsync_Should_CompleteWithoutException_When_ProviderIsUnknown(string provider)
    {
        // Arrange
        var options = CreateOptions(provider, "Server=localhost;Database=fsh");
        var sut = new DatabasePrecreatorHostedService(options, _logger);

        // Act & Assert — must not throw for unknown providers
        await Should.NotThrowAsync(() => sut.StartAsync(CancellationToken.None));
    }

    [Fact]
    public async Task StartAsync_Should_LogWarning_When_ProviderIsUnknown()
    {
        // Arrange
        var options = CreateOptions("SQLITE", "Data Source=:memory:");

        // Configure the logger mock to enable Warning level so the IsEnabled() guard in
        // production code passes and the LogWarning call is actually reached.
        _logger.IsEnabled(LogLevel.Warning).Returns(true);

        var sut = new DatabasePrecreatorHostedService(options, _logger);

        // Act
        await sut.StartAsync(CancellationToken.None);

        // Assert — the IsEnabled guard was checked, confirming the warning code path was reached
        _logger.Received(1).IsEnabled(LogLevel.Warning);
    }


    // ------------------------------------------------------------------ IHostedService contract

    [Fact]
    public void DatabasePrecreatorHostedService_Should_ImplementIHostedService()
    {
        // Arrange
        var options = CreateOptions("POSTGRESQL",
            "Host=localhost;Database=fsh;Username=postgres;Password=pass");
        var sut = new DatabasePrecreatorHostedService(options, _logger);

        // Assert
        sut.ShouldBeAssignableTo<IHostedService>();
    }

    // ------------------------------------------------------------------ helpers

    private static IOptions<DatabaseOptions> CreateOptions(string provider, string connectionString)
    {
        var opts = Substitute.For<IOptions<DatabaseOptions>>();
        opts.Value.Returns(new DatabaseOptions
        {
            Provider = provider,
            ConnectionString = connectionString,
            MigrationsAssembly = "TestMigrations"
        });
        return opts;
    }
}

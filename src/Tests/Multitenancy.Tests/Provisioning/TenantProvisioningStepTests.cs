using FSH.Modules.Multitenancy.Provisioning;

namespace Multitenancy.Tests.Provisioning;

/// <summary>
/// Tests for TenantProvisioningStep - individual provisioning step tracking.
/// </summary>
public sealed class TenantProvisioningStepTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_Should_SetProvisioningId()
    {
        // Arrange
        var provisioningId = Guid.NewGuid();
        var tenantId = "tenant-1";

        // Act
        var step = new TenantProvisioningStep(provisioningId, tenantId, TenantProvisioningStepName.Database);

        // Assert
        step.ProvisioningId.ShouldBe(provisioningId);
    }

    [Fact]
    public void Constructor_Should_SetTenantId()
    {
        // Arrange
        var tenantId = "tenant-1";

        // Act
        var step = new TenantProvisioningStep(Guid.NewGuid(), tenantId, TenantProvisioningStepName.Database);

        // Assert
        step.TenantId.ShouldBe(tenantId);
    }

    [Fact]
    public void Constructor_Should_SetStep()
    {
        // Arrange
        var provisioningId = Guid.NewGuid();

        // Act
        var step = new TenantProvisioningStep(provisioningId, "tenant-1", TenantProvisioningStepName.Migrations);

        // Assert
        step.Step.ShouldBe(TenantProvisioningStepName.Migrations);
    }

    [Fact]
    public void Constructor_Should_SetStatusToPending()
    {
        // Act
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);

        // Assert
        step.Status.ShouldBe(TenantProvisioningStatus.Pending);
    }

    [Fact]
    public void Constructor_Should_GenerateNewId()
    {
        // Act
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);

        // Assert
        step.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Constructor_Should_InitializeNullFields()
    {
        // Act
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);

        // Assert
        step.Error.ShouldBeNull();
        step.StartedOnUtc.ShouldBeNull();
        step.CompletedOnUtc.ShouldBeNull();
    }

    [Theory]
    [InlineData(TenantProvisioningStepName.Database)]
    [InlineData(TenantProvisioningStepName.Migrations)]
    [InlineData(TenantProvisioningStepName.Seeding)]
    [InlineData(TenantProvisioningStepName.CacheWarm)]
    public void Constructor_Should_AcceptAllStepNames(TenantProvisioningStepName stepName)
    {
        // Act
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", stepName);

        // Assert
        step.Step.ShouldBe(stepName);
    }

    #endregion

    #region MarkRunning Tests

    [Fact]
    public void MarkRunning_Should_SetStatusToRunning()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);

        // Act
        step.MarkRunning();

        // Assert
        step.Status.ShouldBe(TenantProvisioningStatus.Running);
    }

    [Fact]
    public void MarkRunning_Should_SetStartedOnUtc_OnFirstCall()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        var before = DateTimeOffset.UtcNow;

        // Act
        step.MarkRunning();
        var after = DateTimeOffset.UtcNow;

        // Assert
        step.StartedOnUtc.ShouldNotBeNull();
        step.StartedOnUtc.Value.ShouldBeGreaterThanOrEqualTo(before);
        step.StartedOnUtc.Value.ShouldBeLessThanOrEqualTo(after);
    }

    [Fact]
    public void MarkRunning_Should_NotOverwriteStartedOnUtc_OnSubsequentCalls()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        step.MarkRunning();
        var firstStartedOnUtc = step.StartedOnUtc;

        // Act - Call again
        step.MarkRunning();

        // Assert - StartedOnUtc should not change (due to ??= operator)
        step.StartedOnUtc.ShouldBe(firstStartedOnUtc);
    }

    #endregion

    #region MarkCompleted Tests

    [Fact]
    public void MarkCompleted_Should_SetStatusToCompleted()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        step.MarkRunning();

        // Act
        step.MarkCompleted();

        // Assert
        step.Status.ShouldBe(TenantProvisioningStatus.Completed);
    }

    [Fact]
    public void MarkCompleted_Should_SetCompletedOnUtc()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        var before = DateTimeOffset.UtcNow;

        // Act
        step.MarkCompleted();
        var after = DateTimeOffset.UtcNow;

        // Assert
        step.CompletedOnUtc.ShouldNotBeNull();
        step.CompletedOnUtc.Value.ShouldBeGreaterThanOrEqualTo(before);
        step.CompletedOnUtc.Value.ShouldBeLessThanOrEqualTo(after);
    }

    #endregion

    #region MarkFailed Tests

    [Fact]
    public void MarkFailed_Should_SetStatusToFailed()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);

        // Act
        step.MarkFailed("Connection failed");

        // Assert
        step.Status.ShouldBe(TenantProvisioningStatus.Failed);
    }

    [Fact]
    public void MarkFailed_Should_SetError()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        var error = "Database connection timeout";

        // Act
        step.MarkFailed(error);

        // Assert
        step.Error.ShouldBe(error);
    }

    [Fact]
    public void MarkFailed_Should_SetCompletedOnUtc()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Database);
        var before = DateTimeOffset.UtcNow;

        // Act
        step.MarkFailed("Error");
        var after = DateTimeOffset.UtcNow;

        // Assert
        step.CompletedOnUtc.ShouldNotBeNull();
        step.CompletedOnUtc.Value.ShouldBeGreaterThanOrEqualTo(before);
        step.CompletedOnUtc.Value.ShouldBeLessThanOrEqualTo(after);
    }

    #endregion

    #region Lifecycle Tests

    [Fact]
    public void Step_Should_SupportSuccessfulLifecycle()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Migrations);
        step.Status.ShouldBe(TenantProvisioningStatus.Pending);

        // Act - Running
        step.MarkRunning();
        step.Status.ShouldBe(TenantProvisioningStatus.Running);
        step.StartedOnUtc.ShouldNotBeNull();

        // Act - Completed
        step.MarkCompleted();
        step.Status.ShouldBe(TenantProvisioningStatus.Completed);
        step.CompletedOnUtc.ShouldNotBeNull();
    }

    [Fact]
    public void Step_Should_SupportFailureLifecycle()
    {
        // Arrange
        var step = new TenantProvisioningStep(Guid.NewGuid(), "tenant-1", TenantProvisioningStepName.Seeding);

        // Act - Running
        step.MarkRunning();
        step.Status.ShouldBe(TenantProvisioningStatus.Running);

        // Act - Failed
        step.MarkFailed("Seeding failed: unique constraint violation");

        // Assert
        step.Status.ShouldBe(TenantProvisioningStatus.Failed);
        step.Error.ShouldNotBeNull();
        step.Error.ShouldContain("unique constraint violation");
        step.CompletedOnUtc.ShouldNotBeNull();
    }

    #endregion
}

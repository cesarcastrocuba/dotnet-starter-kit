using FSH.Modules.Identity.Contracts.DTOs;

namespace Identity.Tests.Services;

/// <summary>
/// Tests for PasswordExpiryStatusDto - the status object returned by PasswordExpiryService.
/// </summary>
public sealed class PasswordExpiryStatusTests
{
    [Fact]
    public void Status_Should_ReturnExpired_When_IsExpiredTrue()
    {
        // Arrange
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = true,
            IsExpiringWithinWarningPeriod = false,
            DaysUntilExpiry = -10,
            ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(-10)
        };

        // Act
        var result = status.Status;

        // Assert
        result.ShouldBe("Expired");
    }

    [Fact]
    public void Status_Should_ReturnExpiringSoon_When_WithinWarningPeriod()
    {
        // Arrange
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = false,
            IsExpiringWithinWarningPeriod = true,
            DaysUntilExpiry = 5,
            ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(5)
        };

        // Act
        var result = status.Status;

        // Assert
        result.ShouldBe("Expiring Soon");
    }

    [Fact]
    public void Status_Should_ReturnValid_When_NotExpiredAndNotExpiringSoon()
    {
        // Arrange
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = false,
            IsExpiringWithinWarningPeriod = false,
            DaysUntilExpiry = 60,
            ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(60)
        };

        // Act
        var result = status.Status;

        // Assert
        result.ShouldBe("Valid");
    }

    [Fact]
    public void Status_Should_PrioritizeExpired_Over_ExpiringSoon()
    {
        // Arrange - Both flags set (edge case)
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = true,
            IsExpiringWithinWarningPeriod = true, // Should be ignored
            DaysUntilExpiry = -1,
            ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(-1)
        };

        // Act
        var result = status.Status;

        // Assert
        result.ShouldBe("Expired"); // Expired takes priority
    }

    [Fact]
    public void Properties_Should_BeSettableAndGettable()
    {
        // Arrange
        var expiresOnUtc = new DateTimeOffset(2024, 12, 31, 12, 0, 0, TimeSpan.Zero);

        // Act
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = true,
            IsExpiringWithinWarningPeriod = false,
            DaysUntilExpiry = -5,
            ExpiresOnUtc = expiresOnUtc
        };

        // Assert
        status.IsExpired.ShouldBeTrue();
        status.IsExpiringWithinWarningPeriod.ShouldBeFalse();
        status.DaysUntilExpiry.ShouldBe(-5);
        status.ExpiresOnUtc.ShouldBe(expiresOnUtc);
    }

    [Fact]
    public void ExpiresOnUtc_Should_AllowNull()
    {
        // Arrange & Act
        var status = new PasswordExpiryStatusDto
        {
            IsExpired = false,
            IsExpiringWithinWarningPeriod = false,
            DaysUntilExpiry = int.MaxValue,
            ExpiresOnUtc = null
        };

        // Assert
        status.ExpiresOnUtc.ShouldBeNull();
        status.Status.ShouldBe("Valid");
    }

    [Fact]
    public void DefaultValues_Should_BeDefaults()
    {
        // Arrange & Act
        var status = new PasswordExpiryStatusDto();

        // Assert
        status.IsExpired.ShouldBeFalse();
        status.IsExpiringWithinWarningPeriod.ShouldBeFalse();
        status.DaysUntilExpiry.ShouldBe(0);
        status.ExpiresOnUtc.ShouldBeNull();
        status.Status.ShouldBe("Valid");
    }
}

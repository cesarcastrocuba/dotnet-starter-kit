using FSH.Modules.Auditing.Contracts.v1.GetAudits;
using FSH.Modules.Auditing.Contracts.v1.GetAuditsByCorrelation;
using FSH.Modules.Auditing.Contracts.v1.GetAuditsByTrace;
using FSH.Modules.Auditing.Contracts.v1.GetAuditSummary;
using FSH.Modules.Auditing.Contracts.v1.GetExceptionAudits;
using FSH.Modules.Auditing.Contracts.v1.GetSecurityAudits;
using FSH.Modules.Auditing.Features.v1.GetAudits;
using FSH.Modules.Auditing.Features.v1.GetAuditsByCorrelation;
using FSH.Modules.Auditing.Features.v1.GetAuditsByTrace;
using FSH.Modules.Auditing.Features.v1.GetAuditSummary;
using FSH.Modules.Auditing.Features.v1.GetExceptionAudits;
using FSH.Modules.Auditing.Features.v1.GetSecurityAudits;
using Shouldly;
using Xunit;

namespace Generic.Tests.Validators;

/// <summary>
/// Tests for generic date range validation rules (FromOnUtc less than or equal to ToOnUtc)
/// that are shared across queries with date filtering.
/// </summary>
public sealed class DateRangeValidatorTests
{
    private static readonly DateTimeOffset BaseDate = new(2024, 1, 15, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void DateRange_Should_Pass_When_BothNull_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery { FromOnUtc = null, ToOnUtc = null };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_BothNull_GetAuditsByCorrelation()
    {
        // Arrange
        var validator = new GetAuditsByCorrelationQueryValidator();
        var query = new GetAuditsByCorrelationQuery { CorrelationId = "test-id", FromOnUtc = null, ToOnUtc = null };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_BothNull_GetAuditsByTrace()
    {
        // Arrange
        var validator = new GetAuditsByTraceQueryValidator();
        var query = new GetAuditsByTraceQuery { TraceId = "test-trace", FromOnUtc = null, ToOnUtc = null };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_BothNull_GetAuditSummary()
    {
        // Arrange
        var validator = new GetAuditSummaryQueryValidator();
        var query = new GetAuditSummaryQuery { FromOnUtc = null, ToOnUtc = null };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_OnlyFromOnUtcSet_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery { FromOnUtc = BaseDate, ToOnUtc = null };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_OnlyToOnUtcSet_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery { FromOnUtc = null, ToOnUtc = BaseDate };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_FromOnUtcEqualsToOnUtc_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery { FromOnUtc = BaseDate, ToOnUtc = BaseDate };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Pass_When_FromOnUtcBeforeToOnUtc_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery
        {
            FromOnUtc = BaseDate,
            ToOnUtc = BaseDate.AddDays(7)
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetAudits()
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery
        {
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetAuditsByCorrelation()
    {
        // Arrange
        var validator = new GetAuditsByCorrelationQueryValidator();
        var query = new GetAuditsByCorrelationQuery
        {
            CorrelationId = "test-id",
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetAuditsByTrace()
    {
        // Arrange
        var validator = new GetAuditsByTraceQueryValidator();
        var query = new GetAuditsByTraceQuery
        {
            TraceId = "test-trace",
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetAuditSummary()
    {
        // Arrange
        var validator = new GetAuditSummaryQueryValidator();
        var query = new GetAuditSummaryQuery
        {
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetExceptionAudits()
    {
        // Arrange
        var validator = new GetExceptionAuditsQueryValidator();
        var query = new GetExceptionAuditsQuery
        {
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Fact]
    public void DateRange_Should_Fail_When_FromOnUtcAfterToOnUtc_GetSecurityAudits()
    {
        // Arrange
        var validator = new GetSecurityAuditsQueryValidator();
        var query = new GetSecurityAuditsQuery
        {
            FromOnUtc = BaseDate.AddDays(7),
            ToOnUtc = BaseDate
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorMessage.Contains("FromOnUtc must be less than or equal to ToOnUtc"));
    }

    [Theory]
    [InlineData(1)]    // 1 second apart
    [InlineData(60)]   // 1 minute apart
    [InlineData(3600)] // 1 hour apart
    public void DateRange_Should_Pass_When_FromOnUtcSlightlyBeforeToOnUtc(int secondsDiff)
    {
        // Arrange
        var validator = new GetAuditsQueryValidator();
        var query = new GetAuditsQuery
        {
            FromOnUtc = BaseDate,
            ToOnUtc = BaseDate.AddSeconds(secondsDiff)
        };

        // Act
        var result = validator.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}

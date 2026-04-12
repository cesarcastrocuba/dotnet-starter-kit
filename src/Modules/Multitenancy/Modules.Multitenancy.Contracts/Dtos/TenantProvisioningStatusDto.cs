namespace FSH.Modules.Multitenancy.Contracts.Dtos;

public sealed record TenantProvisioningStepDto(
    string Step,
    string Status,
    DateTimeOffset? StartedOnUtc,
    DateTimeOffset? CompletedOnUtc,
    string? Error);

public sealed record TenantProvisioningStatusDto(
    string TenantId,
    string Status,
    string CorrelationId,
    string? CurrentStep,
    string? Error,
    DateTimeOffset CreatedOnUtc,
    DateTimeOffset? StartedOnUtc,
    DateTimeOffset? CompletedOnUtc,
    IReadOnlyCollection<TenantProvisioningStepDto> Steps);

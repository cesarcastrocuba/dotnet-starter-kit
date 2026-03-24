namespace FSH.Modules.Multitenancy.Contracts.v1.UpgradeTenant;

public sealed record UpgradeTenantCommandResponse(DateTimeOffset NewValidityOnUtc, string Tenant);
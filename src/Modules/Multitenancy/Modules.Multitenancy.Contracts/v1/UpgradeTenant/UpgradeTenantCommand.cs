using Mediator;

namespace FSH.Modules.Multitenancy.Contracts.v1.UpgradeTenant;

public sealed record UpgradeTenantCommand(string Tenant, DateTimeOffset ExtendedExpiryOnUtc)
    : ICommand<UpgradeTenantCommandResponse>;
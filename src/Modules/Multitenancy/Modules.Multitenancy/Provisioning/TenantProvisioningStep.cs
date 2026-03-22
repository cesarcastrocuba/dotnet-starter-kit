using FSH.Framework.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace FSH.Modules.Multitenancy.Provisioning;

public sealed class TenantProvisioningStep : BaseEntity<Guid>, IHasTenant
{
    public string TenantId { get; private set; } = default!;
    public Guid ProvisioningId { get; private set; }

    public TenantProvisioningStepName Step { get; private set; }

    public TenantProvisioningStatus Status { get; private set; } = TenantProvisioningStatus.Pending;

    public string? Error { get; private set; }

    public DateTimeOffset? StartedOnUtc { get; private set; }

    public DateTimeOffset? CompletedOnUtc { get; private set; }

    [ForeignKey(nameof(ProvisioningId))]
    public TenantProvisioning? Provisioning { get; private set; }

    private TenantProvisioningStep()
    {
    }

    public TenantProvisioningStep(Guid provisioningId, string tenantId, TenantProvisioningStepName step)
    {
        Id = Guid.NewGuid();
        ProvisioningId = provisioningId;
        TenantId = tenantId;
        Step = step;
    }

    public void MarkRunning()
    {
        Status = TenantProvisioningStatus.Running;
        StartedOnUtc ??= DateTimeOffset.UtcNow;
    }

    public void MarkCompleted()
    {
        Status = TenantProvisioningStatus.Completed;
        CompletedOnUtc = DateTimeOffset.UtcNow;
    }

    public void MarkFailed(string error)
    {
        Status = TenantProvisioningStatus.Failed;
        Error = error;
        CompletedOnUtc = DateTimeOffset.UtcNow;
    }
}

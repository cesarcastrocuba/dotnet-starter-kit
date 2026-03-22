using FSH.Framework.Core.Domain;

namespace FSH.Modules.Multitenancy.Provisioning;

public sealed class TenantProvisioning : BaseEntity<Guid>, IHasTenant
{
    public string TenantId { get; private set; } = default!;

    public string CorrelationId { get; private set; } = default!;

    public TenantProvisioningStatus Status { get; private set; } = TenantProvisioningStatus.Pending;

    public string? CurrentStep { get; private set; }

    public string? Error { get; private set; }

    public string? JobId { get; private set; }

    public DateTimeOffset CreatedOnUtc { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedOnUtc { get; private set; }
    public DateTimeOffset? CompletedOnUtc { get; private set; }

    public ICollection<TenantProvisioningStep> Steps { get; private set; } = new List<TenantProvisioningStep>();

    private TenantProvisioning()
    {
    }

    public TenantProvisioning(string tenantId, string correlationId)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CorrelationId = correlationId;
        CreatedOnUtc = DateTimeOffset.UtcNow;
    }

    public void SetJobId(string jobId) => JobId = jobId;

    public void MarkRunning(string step)
    {
        Status = TenantProvisioningStatus.Running;
        StartedOnUtc ??= DateTimeOffset.UtcNow;
        CurrentStep = step;
    }

    public void MarkCompleted()
    {
        Status = TenantProvisioningStatus.Completed;
        CompletedOnUtc = DateTimeOffset.UtcNow;
        CurrentStep = null;
        Error = null;
    }

    public void MarkFailed(string step, string error)
    {
        Status = TenantProvisioningStatus.Failed;
        CurrentStep = step;
        Error = error;
        CompletedOnUtc = DateTimeOffset.UtcNow;
    }
}

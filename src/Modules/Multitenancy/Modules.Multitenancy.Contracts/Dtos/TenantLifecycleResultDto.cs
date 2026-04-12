namespace FSH.Modules.Multitenancy.Contracts.Dtos;

public sealed class TenantLifecycleResultDto
{
    public string TenantId { get; set; } = default!;

    public bool IsActive { get; set; }

    public DateTimeOffset? ValidUptoOnUtc { get; set; }

    public string Message { get; set; } = string.Empty;
}

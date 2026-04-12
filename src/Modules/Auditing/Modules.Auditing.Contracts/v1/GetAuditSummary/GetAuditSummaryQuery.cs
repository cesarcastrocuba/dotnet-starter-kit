using FSH.Modules.Auditing.Contracts.Dtos;
using Mediator;

namespace FSH.Modules.Auditing.Contracts.v1.GetAuditSummary;

public sealed class GetAuditSummaryQuery : IQuery<AuditSummaryAggregateDto>
{
    public DateTimeOffset? FromOnUtc { get; init; }

    public DateTimeOffset? ToOnUtc { get; init; }

    public string? TenantId { get; init; }
}


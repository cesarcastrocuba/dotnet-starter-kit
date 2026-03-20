using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Auditing.Contracts.Dtos;
using FSH.Modules.Auditing.Contracts.v1.GetAuditsByTrace;
using FSH.Modules.Auditing.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Auditing.Features.v1.GetAuditsByTrace;

public sealed class GetAuditsByTraceQueryHandler : IQueryHandler<GetAuditsByTraceQuery, IReadOnlyList<AuditSummaryDto>>
{
    private readonly AuditDbContext _dbContext;

    public GetAuditsByTraceQueryHandler(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<IReadOnlyList<AuditSummaryDto>> Handle(GetAuditsByTraceQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        IQueryable<AuditRecord> audits = _dbContext.AuditRecords
            .AsNoTracking()
            .Where(a => a.TraceId == query.TraceId);

        if (query.FromOnUtc.HasValue)
        {
            audits = audits.Where(a => a.OccurredOnUtc >= query.FromOnUtc.Value);
        }

        if (query.ToOnUtc.HasValue)
        {
            audits = audits.Where(a => a.OccurredOnUtc <= query.ToOnUtc.Value);
        }

        var list = await audits
            .OrderBy(a => a.OccurredOnUtc)
            .Select(a => new AuditSummaryDto
            {
                Id = a.Id,
                OccurredOnUtc = a.OccurredOnUtc,
                EventType = (AuditEventType)a.EventType,
                Severity = (AuditSeverity)a.Severity,
                TenantId = a.TenantId,
                UserId = a.UserId,
                UserName = a.UserName,
                TraceId = a.TraceId,
                CorrelationId = a.CorrelationId,
                RequestId = a.RequestId,
                Source = a.Source,
                Tags = (AuditTag)a.Tags
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return list;
    }
}


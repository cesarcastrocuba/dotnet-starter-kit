using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Auditing.Contracts.Dtos;
using FSH.Modules.Auditing.Contracts.v1.GetSecurityAudits;
using FSH.Modules.Auditing.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Auditing.Features.v1.GetSecurityAudits;

public sealed class GetSecurityAuditsQueryHandler : IQueryHandler<GetSecurityAuditsQuery, IReadOnlyList<AuditSummaryDto>>
{
    private readonly AuditDbContext _dbContext;

    public GetSecurityAuditsQueryHandler(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<IReadOnlyList<AuditSummaryDto>> Handle(GetSecurityAuditsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        IQueryable<AuditRecord> audits = _dbContext.AuditRecords
            .AsNoTracking()
            .Where(a => a.EventType == (int)AuditEventType.Security);

        if (query.FromOnUtc.HasValue)
        {
            audits = audits.Where(a => a.OccurredOnUtc >= query.FromOnUtc.Value);
        }

        if (query.ToOnUtc.HasValue)
        {
            audits = audits.Where(a => a.OccurredOnUtc <= query.ToOnUtc.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.UserId))
        {
            audits = audits.Where(a => a.UserId == query.UserId);
        }

        if (!string.IsNullOrWhiteSpace(query.TenantId))
        {
            audits = audits.Where(a => a.TenantId == query.TenantId);
        }

        if (query.Action.HasValue && query.Action.Value != SecurityAction.None)
        {
            string actionValue = query.Action.Value.ToString();
            audits = audits.Where(a => a.PayloadJson != null &&
                EF.Functions.ILike(a.PayloadJson, $"%\"action\":\"{actionValue}\"%"));
        }

        var list = await audits
            .OrderByDescending(a => a.OccurredOnUtc)
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


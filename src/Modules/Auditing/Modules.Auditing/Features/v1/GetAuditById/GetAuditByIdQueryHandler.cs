using System.Text.Json;
using FSH.Modules.Auditing.Contracts;
using FSH.Modules.Auditing.Contracts.Dtos;
using FSH.Modules.Auditing.Contracts.v1.GetAuditById;
using FSH.Modules.Auditing.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Auditing.Features.v1.GetAuditById;

public sealed class GetAuditByIdQueryHandler : IQueryHandler<GetAuditByIdQuery, AuditDetailDto>
{
    private readonly AuditDbContext _dbContext;

    public GetAuditByIdQueryHandler(AuditDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<AuditDetailDto> Handle(GetAuditByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var record = await _dbContext.AuditRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == query.Id, cancellationToken)
            .ConfigureAwait(false);

        if (record is null)
        {
            throw new KeyNotFoundException($"Audit record {query.Id} not found.");
        }

        JsonElement payload;
        try
        {
            using var document = JsonDocument.Parse(record.PayloadJson);
            payload = document.RootElement.Clone();
        }
        catch
        {
            payload = JsonDocument.Parse("{}").RootElement.Clone();
        }

        return new AuditDetailDto
        {
            Id = record.Id,
            OccurredOnUtc = record.OccurredOnUtc,
            ReceivedOnUtc = record.ReceivedOnUtc,
            EventType = (AuditEventType)record.EventType,
            Severity = (AuditSeverity)record.Severity,
            TenantId = record.TenantId,
            UserId = record.UserId,
            UserName = record.UserName,
            TraceId = record.TraceId,
            SpanId = record.SpanId,
            CorrelationId = record.CorrelationId,
            RequestId = record.RequestId,
            Source = record.Source,
            Tags = (AuditTag)record.Tags,
            Payload = payload
        };
    }
}


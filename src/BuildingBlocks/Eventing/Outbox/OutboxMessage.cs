using FSH.Framework.Core.Domain;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Framework.Eventing.Outbox;

public sealed class OutboxMessage : IHasTenant
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? ProcessedOnUtc { get; set; }
    public string? LastError { get; set; }
    public int RetryCount { get; set; }
    public bool IsDead { get; set; }
    public string TenantId { get; set; } = default!;
    public string CorrelationId { get; set; } = default!;
}

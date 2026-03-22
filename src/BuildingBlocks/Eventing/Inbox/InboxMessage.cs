using FSH.Framework.Core.Domain;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Framework.Eventing.Inbox;

public sealed class InboxMessage : IHasTenant
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = default!;
    public string HandlerName { get; set; } = default!;
    public DateTimeOffset ProcessedOnUtc { get; set; }
    public string TenantId { get; set; } = default!;
}

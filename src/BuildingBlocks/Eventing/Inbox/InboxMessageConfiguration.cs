using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Framework.Eventing.Inbox;

public sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    private readonly string _schema;

    public InboxMessageConfiguration(string schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("InboxMessages", _schema);

        builder.IsMultiTenant();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType).HasMaxLength(256).IsRequired();
        builder.Property(x => x.HandlerName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.TenantId).HasMaxLength(64).IsRequired();

        builder.HasIndex(x => new { x.Id, x.HandlerName, x.TenantId }).IsUnique();
    }
}

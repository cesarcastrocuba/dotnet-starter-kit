using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Framework.Eventing.Outbox;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    private readonly string _schema;

    public OutboxMessageConfiguration(string schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("OutboxMessages", _schema);

        builder.IsMultiTenant();

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Payload).IsRequired();
        builder.Property(x => x.TenantId).HasMaxLength(64).IsRequired();
        builder.Property(x => x.CorrelationId).HasMaxLength(64).IsRequired();

        builder.HasIndex(x => new { x.CreatedOnUtc, x.ProcessedOnUtc, x.IsDead })
            .HasFilter("\"ProcessedOnUtc\" IS NULL AND \"IsDead\" = FALSE");
    }
}

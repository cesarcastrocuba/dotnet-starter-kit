using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Provisioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;

namespace FSH.Modules.Multitenancy.Data.Configurations;

public class TenantProvisioningStepConfiguration : IEntityTypeConfiguration<TenantProvisioningStep>
{
    public void Configure(EntityTypeBuilder<TenantProvisioningStep> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("TenantProvisioningSteps", MultitenancyConstants.Schema);

        builder.IsMultiTenant();

        builder.Property(x => x.TenantId)
            .HasMaxLength(64)
            .IsRequired();
    }
}

using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore.Stores;
using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Domain;
using FSH.Modules.Multitenancy.Provisioning;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Multitenancy.Data;

public class TenantDbContext : EFCoreStoreDbContext<AppTenantInfo>, IMultiTenantDbContext
{
    public const string Schema = "tenant";
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _multiTenantContextAccessor;

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor)
        : base(options)
    {
        _multiTenantContextAccessor = multiTenantContextAccessor;
    }

    ITenantInfo? IMultiTenantDbContext.TenantInfo => _multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
    public TenantMismatchMode TenantMismatchMode { get; set; } = TenantMismatchMode.Ignore;
    public TenantNotSetMode TenantNotSetMode { get; set; } = TenantNotSetMode.Throw;

    public DbSet<TenantProvisioning> TenantProvisionings => Set<TenantProvisioning>();

    public DbSet<TenantProvisioningStep> TenantProvisioningSteps => Set<TenantProvisioningStep>();

    public DbSet<TenantTheme> TenantThemes => Set<TenantTheme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
    }
}

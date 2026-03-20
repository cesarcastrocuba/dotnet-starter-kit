using FSH.Framework.Core.Domain;

namespace FSH.Modules.Identity.Domain;

public class GroupRole : IHasTenant
{
    public string TenantId { get; private set; } = default!;
    public Guid GroupId { get; private set; }
    public string RoleId { get; private set; } = default!;

    // Navigation properties (init for EF Core materialization)
    public virtual Group? Group { get; init; }
    public virtual FshRole? Role { get; init; }

    private GroupRole() { } // EF Core

    public static GroupRole Create(Guid groupId, string roleId, string? tenantId = null)
    {
        return new GroupRole
        {
            TenantId = tenantId!,
            GroupId = groupId,
            RoleId = roleId
        };
    }
}

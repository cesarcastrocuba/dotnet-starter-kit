using FSH.Framework.Core.Domain;

namespace FSH.Modules.Identity.Domain;

public class UserGroup : IHasTenant
{
    public string TenantId { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public Guid GroupId { get; private set; }
    public DateTimeOffset AddedAtOnUtc { get; private set; }
    public string? AddedBy { get; private set; }

    // Navigation properties (init for EF Core materialization)
    public virtual FshUser? User { get; init; }
    public virtual Group? Group { get; init; }

    private UserGroup() { } // EF Core

    public static UserGroup Create(string userId, Guid groupId, string? addedBy = null, string? tenantId = null)
    {
        return new UserGroup
        {
            TenantId = tenantId!,
            UserId = userId,
            GroupId = groupId,
            AddedAtOnUtc = DateTimeOffset.UtcNow,
            AddedBy = addedBy
        };
    }
}

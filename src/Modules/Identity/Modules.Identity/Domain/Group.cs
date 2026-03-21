using FSH.Framework.Core.Domain;
using FSH.Framework.Shared.Multitenancy;

namespace FSH.Modules.Identity.Domain;

public class Group : BaseEntity<Guid>, IAuditableEntity, ISoftDeletable, IHasTenant
{
    public string TenantId { get; set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsSystemGroup { get; private set; }

    // IAuditableEntity implementation
    public DateTimeOffset CreatedOnUtc { get; internal set; }
    public string? CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedOnUtc { get; internal set; }
    public string? LastModifiedBy { get; internal set; }

    // ISoftDeletable implementation
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOnUtc { get; internal set; }
    public string? DeletedBy { get; internal set; }

    // Navigation properties
    public virtual ICollection<GroupRole> GroupRoles { get; private set; } = [];
    public virtual ICollection<UserGroup> UserGroups { get; private set; } = [];

    private Group() { } // EF Core

    public static Group Create(string name, string tenantId, string? description = null, bool isDefault = false, bool isSystemGroup = false, string? createdBy = null)
    {
        return new Group
        {
            Id = Guid.NewGuid(),
            Name = name,
            TenantId = tenantId,
            Description = description,
            IsDefault = isDefault,
            IsSystemGroup = isSystemGroup,
            CreatedBy = createdBy
        };
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void SetAsDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}

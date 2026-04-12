using FSH.Framework.Core.Domain;

namespace FSH.Modules.Identity.Domain;

public class PasswordHistory : IHasTenant
{
    public int Id { get; init; }
    public string TenantId { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; private set; }

    // Navigation property (init for EF Core materialization)
    public virtual FshUser? User { get; init; }

    private PasswordHistory() { } // EF Core

    public static PasswordHistory Create(string userId, string passwordHash, string? tenantId = null)
    {
        return new PasswordHistory
        {
            TenantId = tenantId!,
            UserId = userId,
            PasswordHash = passwordHash,
            CreatedOnUtc = DateTimeOffset.UtcNow
        };
    }
}

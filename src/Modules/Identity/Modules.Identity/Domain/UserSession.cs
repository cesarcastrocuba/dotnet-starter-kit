using FSH.Framework.Core.Domain;
using FSH.Modules.Identity.Domain.Events;

namespace FSH.Modules.Identity.Domain;

[IgnoreAuditTrail]
public class UserSession : BaseEntity<Guid>, IHasTenant
{
    public string TenantId { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public string RefreshTokenHash { get; private set; } = default!;
    public string IpAddress { get; private set; } = default!;
    public string UserAgent { get; private set; } = default!;
    public string? DeviceType { get; private set; }
    public string? Browser { get; private set; }
    public string? BrowserVersion { get; private set; }
    public string? OperatingSystem { get; private set; }
    public string? OsVersion { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public DateTimeOffset LastActivityOnUtc { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedOnUtc { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevokedReason { get; private set; }

    // Navigation property (init for EF Core materialization)
    public virtual FshUser? User { get; init; }

    private UserSession() { } // EF Core

    public static UserSession Create(
        string userId,
        string refreshTokenHash,
        string ipAddress,
        string userAgent,
        DateTimeOffset expiresOnUtc,
        string? deviceType = null,
        string? browser = null,
        string? browserVersion = null,
        string? operatingSystem = null,
        string? osVersion = null,
        string? tenantId = null)
    {
        return new UserSession
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId!,
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            DeviceType = deviceType,
            Browser = browser,
            BrowserVersion = browserVersion,
            OperatingSystem = operatingSystem,
            OsVersion = osVersion,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastActivityOnUtc = DateTimeOffset.UtcNow,
            ExpiresOnUtc = expiresOnUtc
        };
    }

    public void UpdateActivity()
    {
        LastActivityOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateRefreshToken(string refreshTokenHash, DateTimeOffset expiresOnUtc)
    {
        RefreshTokenHash = refreshTokenHash;
        ExpiresOnUtc = expiresOnUtc;
        LastActivityOnUtc = DateTimeOffset.UtcNow;
    }

    public void Revoke(string? revokedBy = null, string? reason = null, string? tenantId = null)
    {
        if (IsRevoked) return;
        IsRevoked = true;
        RevokedOnUtc = DateTimeOffset.UtcNow;
        RevokedBy = revokedBy;
        RevokedReason = reason;

        AddDomainEvent(SessionRevokedEvent.Create(
            userId: UserId,
            sessionId: Id,
            revokedBy: revokedBy,
            reason: reason,
            tenantId: tenantId));
    }
}

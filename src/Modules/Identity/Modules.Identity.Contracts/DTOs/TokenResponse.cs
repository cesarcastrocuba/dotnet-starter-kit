namespace FSH.Modules.Identity.Contracts.DTOs;

public sealed record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresOnUtc,
    DateTimeOffset AccessTokenExpiresOnUtc);
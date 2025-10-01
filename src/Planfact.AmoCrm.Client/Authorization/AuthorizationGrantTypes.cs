
namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Типы grant_type для OAuth 2.0 авторизации в amoCRM API
/// </summary>
internal static class AuthorizationGrantTypes
{
    /// <summary>
    /// Авторизация по коду доступа. Используется для первичного получения Access Token
    /// </summary>
    public const string AuthorizationCode = "authorization_code";

    /// <summary>
    /// Авторизация по refresh token. Используется для обновления Access Token
    /// </summary>
    public const string RefreshToken = "refresh_token";
}

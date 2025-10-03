
namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Контракт сервиса авторизации в amoCRM
/// </summary>
public interface IAmoCrmAuthorizationService
{
    /// <summary>
    /// Получение авторизационных данных интеграции с amoCRM
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM, к которому привязана интеграция</param>
    /// <param name="authorizationCode">Одноразовый код авторизации, предназначенный для обмена на токены доступа</param>
    /// <param name="redirectUri">URI, на который должны перенаправляться входящие запросы из amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий токены доступа OAuth 2.0</returns>
    public Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление авторизационных данных интеграции с amoCRM
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM, к которому привязана интеграция</param>
    /// <param name="refreshToken">Токен обновления авторизационных данных по протоколу OAuth 2.0</param>
    /// <param name="redirectUri">URI, на который должны перенаправляться входящие запросы из amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий токены доступа OAuth 2.0</returns>
    public Task<AuthorizationTokens> RefreshTokenAsync(
        string subdomain,
        string refreshToken,
        string redirectUri,
        CancellationToken cancellationToken = default);
}

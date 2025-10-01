using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Authorization;

namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Запрос в API amoCRM на получение авторизационных данных по токену обновления OAuth 2.0
/// </summary>
public sealed record RefreshTokenRequest
{
    /// <summary>
    /// ID интеграции c amoCRM
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; init; } = string.Empty;

    /// <summary>
    /// Секретный ключ интеграции
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Тип авторизационных данных
    /// </summary>
    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = AuthorizationGrantTypes.RefreshToken;

    /// <summary>
    /// Токен обновления
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>
    /// Uri для получения авторизационных данных после подключения интеграции с amoCRM
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; init; }
}

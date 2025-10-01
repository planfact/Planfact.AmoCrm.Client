using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Authorization;

namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Запрос в API amoCRM на получение авторизационных данных по коду авторизации
/// </summary>
public sealed record AuthorizationRequest
{
    /// <summary>
    /// ID интеграции c amoCRM
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; init; } = string.Empty;

    /// <summary>
    /// Секретный ключ интеграции c amoCRM
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Тип авторизационных данных
    /// </summary>
    [JsonPropertyName("grant_type")]
    public string GrantType { get; } = AuthorizationGrantTypes.AuthorizationCode;

    /// <summary>
    /// Код авторизации
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Uri для получения авторизационных данных после подключения интеграции c amoCRM
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; init; }
}


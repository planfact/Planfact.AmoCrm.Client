using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Authorization;

/// <summary>
/// Ответ API amoCRM, содержащий авторизационные данные
/// </summary>
public sealed record AuthorizationTokens : AmoCrm.Client.Common.BaseResponse
{
    /// <summary>
    /// Срок действия токена доступа в секундах
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Токен доступа
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Токен обновления
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
}

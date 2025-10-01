using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о пользователе
/// </summary>
public sealed record User
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    [JsonPropertyName("name")]
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// E-mail пользователя
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Язык пользователя (ru, en, es)
    /// </summary>
    [JsonPropertyName("lang")]
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Права пользователя
    /// </summary>
    [JsonPropertyName("rights")]
    public UserPermissions? Permissions { get; init; }

    /// <summary>
    /// Дополнительная информация о пользователе (роли и группы, если запрошены через with)
    /// </summary>
    [JsonPropertyName("_embedded")]
    public UserDetails? Details { get; init; }
}

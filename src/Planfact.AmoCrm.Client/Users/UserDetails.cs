using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Дополнительная информация о пользователе amoCRM
/// </summary>
public sealed record UserDetails
{
    /// <summary>
    /// Роли пользователя
    /// </summary>
    [JsonPropertyName("roles")]
    public UserRole[]? Roles { get; init; }

    /// <summary>
    /// Группы пользователя
    /// </summary>
    [JsonPropertyName("groups")]
    public UserGroup[]? Groups { get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Роль пользователя amoCRM
/// </summary>
public sealed record UserRole
{
    /// <summary>
    /// ID роли
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название роли
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Ссылка на роль
    /// </summary>
    [JsonPropertyName("_links")]
    public AmoCrm.Client.Common.LinksResponse SelfLink { get; init; } = null!;
}

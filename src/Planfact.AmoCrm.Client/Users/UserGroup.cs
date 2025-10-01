using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Группа, в которой состоит пользователь amoCRM
/// </summary>
public sealed record UserGroup
{
    /// <summary>
    /// ID группы
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название группы
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}

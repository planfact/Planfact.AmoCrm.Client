
using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель тега в amoCRM
/// </summary>
public sealed record Tag
{
    /// <summary>
    /// Идентификатор тега
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    /// <summary>
    /// Название тега
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Цвет тега
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }
}

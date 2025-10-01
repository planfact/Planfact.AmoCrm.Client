using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Коллекция тегов
/// </summary>
public record TagsContainer
{
    /// <summary>
    /// Массив тегов сущности
    /// </summary>
    [JsonPropertyName("tags")]
    public Tag[] Tags { get; init; } = [];
}

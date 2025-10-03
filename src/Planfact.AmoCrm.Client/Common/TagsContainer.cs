using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель вложенных тегов в API amoCRM
/// </summary>
public record TagsContainer
{
    /// <summary>
    /// Теги
    /// </summary>
    [JsonPropertyName("tags")]
    public Tag[]? Tags { get; init; }
}

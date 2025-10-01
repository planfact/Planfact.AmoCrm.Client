using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель связей конкретной сущности в amoCRM
/// </summary>
public sealed record LinksResponse
{
    /// <summary>
    /// URL самой сущности
    /// </summary>
    [JsonPropertyName("self")]
    public Link? Self { get; init; }

    /// <summary>
    /// URL следующей сущности. Используется при пагинации
    /// </summary>
    [JsonPropertyName("next")]
    public Link? Next { get; init; }

    /// <summary>
    /// URL первой сущности в списке. Используется при пагинации
    /// </summary>
    [JsonPropertyName("first")]
    public Link? First { get; init; }

    /// <summary>
    /// URL предыдущей сущности. Используется при пагинации
    /// </summary>
    [JsonPropertyName("prev")]
    public Link? Prev { get; init; }
}

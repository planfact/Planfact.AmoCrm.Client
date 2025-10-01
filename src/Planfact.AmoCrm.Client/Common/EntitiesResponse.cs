using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Базовый ответ от API сущностей amoCRM
/// </summary>
public record EntitiesResponse : BaseResponse
{
    /// <summary>
    /// URL связанных сущностей того же типа
    /// </summary>
    [JsonPropertyName("_links")]
    public LinksResponse? Links { get; init; }

    /// <summary>
    /// Вложенные сущности
    /// </summary>
    [JsonPropertyName("_embedded")]
    public EmbeddedEntitiesResponse? Embedded { get; init; }
}

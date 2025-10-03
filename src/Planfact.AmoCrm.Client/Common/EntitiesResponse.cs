using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Базовый ответ от API сущностей amoCRM
/// </summary>
public record EntitiesResponse : BaseResponse
{
    /// <summary>
    /// Ссылки для пагинации
    /// </summary>
    [JsonPropertyName("_links")]
    public PaginationLinksResponse? PaginationLinks { get; init; }

    /// <summary>
    /// Вложенные сущности
    /// </summary>
    [JsonPropertyName("_embedded")]
    public EmbeddedEntitiesResponse? Embedded { get; init; }
}

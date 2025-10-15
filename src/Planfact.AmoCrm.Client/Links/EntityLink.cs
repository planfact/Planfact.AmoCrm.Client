using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о связи сущностей
/// </summary>
public sealed record EntityLink : BaseResponse
{
    /// <summary>
    /// Идентификатор главной сущности
    /// </summary>
    [JsonPropertyName("entity_id")]
    public int EntityId { get; init; }

    /// <summary>
    /// Тип главной сущности
    /// </summary>
    [JsonPropertyName("entity_type")]
    public EntityType EntityType { get; init; }

    /// <summary>
    /// Идентификатор связанной сущности
    /// </summary>
    [JsonPropertyName("to_entity_id")]
    public int LinkedEntityId { get; init; }

    /// <summary>
    /// Тип связанной сущности
    /// </summary>
    [JsonPropertyName("to_entity_type")]
    public EntityType LinkedEntityType { get; init; }

    /// <summary>
    /// Метаданные связанной сущности
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public LinkedEntityMetadata? Metadata { get; init; }
}

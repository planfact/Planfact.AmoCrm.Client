using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Модель запроса на создание связи между сущностями в amoCRM
/// </summary>
public sealed record LinkEntitiesRequest
{
    /// <summary>
    /// Идентификатор главной сущности
    /// </summary>
    [JsonPropertyName("entity_id")]
    public int EntityId { get; init; }

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

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    /// <param name="entityId">Идентификатор главной сущности</param>
    public LinkEntitiesRequest(int entityId)
    {
        EntityId = entityId;
    }
}

using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Модель запроса на создание задачи в API amoCRM
/// </summary>
public sealed record AddTaskRequest
{
    /// <summary>
    /// Идентификатор пользователя, ответственного за задачу
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего задачу
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего задачу
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания задачи в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования задачи в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Идентификатор сущности, к которой привязана задача
    /// </summary>
    [JsonPropertyName("entity_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EntityId { get; init; }

    /// <summary>
    /// Тип сущности, к которой привязана задача
    /// </summary>
    [JsonPropertyName("entity_type")]
    public string? EntityTypeName { get; }

    /// <summary>
    /// Признак выполнения задачи
    /// </summary>
    [JsonPropertyName("is_completed")]
    public bool? IsCompleted { get; init; }

    /// <summary>
    /// Идентификатор типа задачи
    /// </summary>
    [JsonPropertyName("task_type_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TaskTypeId { get; init; }

    /// <summary>
    /// Описание задачи
    /// </summary>
    [JsonPropertyName("text")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Длительность задачи в секундах
    /// </summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; init; }

    /// <summary>
    /// Дата, до которой задача должна быть завершена в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("complete_till")]
    public long CompleteTill { get; init; }

    /// <summary>
    /// Результат выполнения задачи
    /// </summary>
    [JsonPropertyName("result")]
    public TaskResult? Result { get; init; }

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    /// <param name="description">Описание задачи</param>
    /// <param name="completeTill">Дата, до которой задача должна быть завершена в формате Unix Timestamp</param>
    /// <param name="entityType">Тип сущности, к которой привязана задача</param>
    public AddTaskRequest(
        string description,
        long completeTill,
        EntityType entityType)
    {
        Description = description;
        CompleteTill = completeTill;
        EntityTypeName = EntityTypeConverter.ToString(entityType);
    }
}

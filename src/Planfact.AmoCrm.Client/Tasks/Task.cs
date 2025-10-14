using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о задаче
/// </summary>
public sealed record Task
{
    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за задачу
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор группы, в которой состоит ответственный за задачу пользователь
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? ResponsibleUserGroupId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего задачу
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего задачу
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания задачи в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования задачи в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// ID сущности, к которой привязана задача
    /// </summary>
    [JsonPropertyName("entity_id")]
    public int? EntityId { get; init; }

    /// <summary>
    /// Тип сущности, к которой привязана задача
    /// </summary>
    [JsonPropertyName("entity_type")]
    public EntityType? EntityType { get; init; }

    /// <summary>
    /// Признак выполнения задачи
    /// </summary>
    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Тип задачи
    /// </summary>
    [JsonPropertyName("task_type_id")]
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
    public int? Duration { get; init; }

    /// <summary>
    ///Дата, до которой задача должна быть завершена в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("complete_till")]
    public long CompleteTill { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится задача
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Результат выполнения задачи
    /// </summary>
    [JsonPropertyName("result")]
    [JsonConverter(typeof(TaskResultConverter))]
    public TaskResult? Result { get; init; }
}

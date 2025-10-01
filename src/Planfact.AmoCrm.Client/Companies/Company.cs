using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Companies;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о компании
/// </summary>
public sealed record Company : AmoCrm.Client.Common.EntitiesResponse
{
    /// <summary>
    /// Идентификатор компании
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название компании
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за компанию
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор группы, в которой состоит ответственный за компанию пользователь
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? ResponsibleUserGroupId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего компанию
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего компанию
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания компании в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования компании в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Срок выполнения ближайшей задачи, связанной с компанией, в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("closest_task_at")]
    public long? ClosestTaskAt { get; init; }

    /// <summary>
    /// Признак удаления компании
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится компания
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для компании
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }
}

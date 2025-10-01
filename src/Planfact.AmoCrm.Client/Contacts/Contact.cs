using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Contacts;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о контакте
/// </summary>
public sealed record Contact : AmoCrm.Client.Common.EntitiesResponse
{
    /// <summary>
    /// Идентификатор контакта
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Полное имя контакта
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Имя контакта
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Фамилия контакта
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    /// <summary>
    /// Идентификатор группы, в которой состоит ответственный пользователь за контакт
    /// </summary>
    [JsonPropertyName("group_id")]
    public int ResponsibleUserGroupId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за контакт
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего контакт
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего контакт
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Unix Timestamp создания контакта
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Unix Timestamp редактирования контакта
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Срок выполнения ближайшей задачи, связанной с контактом, в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("closest_task_at")]
    public long? ClosestTaskAt { get; init; }

    /// <summary>
    /// Признак удаления контакта
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для контакта
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится контакт
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Contacts;

/// <summary>
/// Модель запроса на создание контакта в API amoCRM
/// </summary>
public sealed record AddContactRequest
{
    /// <summary>
    /// Полное имя контакта
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Имя контакта
    /// </summary>
    [JsonPropertyName("first_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Фамилия контакта
    /// </summary>
    [JsonPropertyName("last_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? LastName { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за контакт
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего контакт
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего контакт
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания контакта в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования контакта в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для контакта
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Теги, которые должны быть добавлены к созданному контакту
    /// </summary>
    [JsonPropertyName("tags_to_add")]
    public AmoCrm.Client.Common.Tag[]? TagsToAdd { get; init; }

    /// <summary>
    /// Вложенные сущности
    /// </summary>
    [JsonPropertyName("_embedded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AmoCrm.Client.Common.EmbeddedEntitiesRequest? Embedded { get; init; }
}

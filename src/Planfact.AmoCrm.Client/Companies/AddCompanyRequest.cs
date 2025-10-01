using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Companies;

/// <summary>
/// Модель запроса на создание компании в API amoCRM
/// </summary>
public sealed record AddCompanyRequest
{
    /// <summary>
    /// Название компании
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за компанию
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего компанию
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего компанию
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания компании в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования компании в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для компании
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Теги, которые должны быть добавлены к созданной компании
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

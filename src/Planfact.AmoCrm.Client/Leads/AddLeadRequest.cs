using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Leads;

/// <summary>
/// Модель запроса на создание сделки в amoCRM
/// </summary>
public sealed record AddLeadRequest
{
    /// <summary>
    /// Название сделки
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    /// Бюджет сделки
    /// </summary>
    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Price { get; init; }

    /// <summary>
    /// Статус сделки
    /// </summary>
    [JsonPropertyName("status_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StatusId { get; init; }

    /// <summary>
    /// Идентификатор воронки, в которой находится сделка
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PipelineId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего сделку
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего сделку
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата закрытия сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("closed_at")]
    public long? ClosedAt { get; init; }

    /// <summary>
    /// Дата создания сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Идентификатор причины отказа клиента от сделки
    /// </summary>
    [JsonPropertyName("loss_reason_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LossReasonId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за сделку
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для сделки
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Теги, которые должны быть добавлены к созданной сделке
    /// </summary>
    [JsonPropertyName("tags_to_add")]
    public AmoCrm.Client.Common.Tag[]? TagsToAdd { get; init; }

    /// <summary>
    /// Вложенные сущности
    /// </summary>
    [JsonPropertyName("_embedded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AmoCrm.Client.Common.EmbeddedEntitiesRequest? Embedded {  get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Leads;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о сделке
/// </summary>
public sealed record Lead : AmoCrm.Client.Common.EntitiesResponse
{
    /// <summary>
    /// Идентификатор сделки
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Бюджет сделки
    /// </summary>
    [JsonPropertyName("price")]
    public int Price { get; init; }

    /// <summary>
    /// Название сделки
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за сделку
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор группы, в которой состоит пользователь, ответственный за сделку
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? ResponsibleUserGroupId { get; init; }

    /// <summary>
    /// Идентификатор воронки, в которой находится сделка
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; init; }

    /// <summary>
    /// Идентификатор причины отказа клиента от сделки
    /// </summary>
    [JsonPropertyName("loss_reason_id")]
    public int? LossReasonId { get; init; }

    /// <summary>
    /// Идентификатор статуса сделки
    /// </summary>
    [JsonPropertyName("status_id")]
    public int StatusId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего сделку
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего сделку
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Дата закрытия сделки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("closed_at")]
    public long? ClosedAt { get; init; }

    /// <summary>
    /// Срок выполнения ближайшей задачи, связанной со сделкой, в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("closest_task_at")]
    public long? ClosestTaskAt { get; init; }

    /// <summary>
    /// Признак удаления сделки
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для сделки
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public AmoCrm.Client.CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Скоринг сделки
    /// </summary>
    [JsonPropertyName("score")]
    public int? Score { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится сделка
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Трудозатраты на работу со сделкой в секундах
    /// </summary>
    [JsonPropertyName("labor_cost")]
    public int? LaborCost { get; init; }
}

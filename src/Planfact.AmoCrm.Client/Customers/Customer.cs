using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Customers;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о покупателе
/// </summary>
public sealed record Customer : Common.EntitiesResponse
{
    /// <summary>
    /// Идентификатор покупателя
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название покупателя
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Ожидаемая сумма покупки
    /// </summary>
    [JsonPropertyName("next_price")]
    public int? NextPrice { get; init; }

    /// <summary>
    /// Ожидаемая дата следующей покупки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("next_date")]
    public long? NextDate { get; init; }

    /// <summary>
    /// Идентификатор ответственного пользователя
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор статуса покупателя в аккаунте
    /// <see href="https://www.amocrm.ru/developers/content/crm_platform/customers-statuses-api" />
    /// </summary>
    [JsonPropertyName("status_id")]
    public int? StatusId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего покупателя
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего покупателя
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания покупателя в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования покупателя в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Unix Timestamp ближайшей задачи к выполнению
    /// </summary>
    [JsonPropertyName("closest_task_at")]
    public long? ClosestTaskAt { get; init; }

    /// <summary>
    /// Признак удаления покупателя
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Совокупная сумма покупок
    /// </summary>
    [JsonPropertyName("ltv")]
    public int? TotalSpent { get; init; }

    /// <summary>
    /// Количество покупок
    /// </summary>
    [JsonPropertyName("purchases_count")]
    public int? PurchasesCount { get; init; }

    /// <summary>
    /// Средняя сумма покупки
    /// </summary>
    [JsonPropertyName("average_check")]
    public int? AverageCheck { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится покупатель
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для покупателя
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }
}

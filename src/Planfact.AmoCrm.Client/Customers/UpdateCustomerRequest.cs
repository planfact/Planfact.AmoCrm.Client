using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Customers;

/// <summary>
/// Модель запроса на редактирование покупателя в API amoCRM
/// </summary>
public sealed record UpdateCustomerRequest
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
    /// Ожидаемая сумма следующей покупки
    /// </summary>
    [JsonPropertyName("next_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NextPrice { get; init; }

    /// <summary>
    /// Ожидаемая дата следующей покупки в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("next_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? NextDate { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за покупателя
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор статуса покупателя в аккаунте
    /// <see href="https://www.amocrm.ru/developers/content/crm_platform/customers-statuses-api" />
    /// </summary>
    [JsonPropertyName("status_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StatusId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего покупателя
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего покупателя
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания покупателя в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования покупателя в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Значения дополнительных полей, заполненных для покупателя
    /// </summary>
    [JsonPropertyName("custom_fields_values")]
    public CustomFields.CustomFieldValuesContainer[]? CustomFieldValues { get; init; }

    /// <summary>
    /// Теги, которые должны быть добавлены к покупателю
    /// </summary>
    [JsonPropertyName("tags_to_add")]
    public Tag[]? TagsToAdd { get; init; }

    /// <summary>
    /// Теги, которые должны быть удалены из покупателя
    /// </summary>
    [JsonPropertyName("tags_to_delete")]
    public Tag[]? TagsToDelete { get; init; }

    /// <summary>
    /// Вложенные сущности
    /// </summary>
    [JsonPropertyName("_embedded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EmbeddedEntitiesRequest? Embedded { get; init; }

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    /// <param name="id">Идентификатор покупателя</param>
    /// <param name="name">Название покупателя</param>
    public UpdateCustomerRequest(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Transactions;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о транзакции
/// </summary>
public sealed record Transaction : Common.EntitiesResponse
{
    /// <summary>
    /// Идентификатор транзакции
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Комментарий к покупке
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; init; }

    /// <summary>
    /// Сумма покупки
    /// </summary>
    [JsonPropertyName("price")]
    public int Price { get; init; }

    /// <summary>
    /// Дата проведения транзакции в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("completed_at")]
    public long? CompletedAt { get; init; }

    /// <summary>
    /// Идентификатор покупателя, к которому привязана транзакция
    /// </summary>
    [JsonPropertyName("customer_id")]
    public int CustomerId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего транзакцию
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего транзакцию
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания транзакции в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования транзакции в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Признак удаления транзакции
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Идентификатор аккаунта, в котором находится транзакция
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Transactions;

/// <summary>
/// Модель запроса на создание транзакции в API amoCRM
/// </summary>
public sealed record AddTransactionRequest
{
    /// <summary>
    /// Комментарий к покупке
    /// </summary>
    [JsonPropertyName("comment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Comment { get; init; }

    /// <summary>
    /// Сумма покупки
    /// </summary>
    [JsonPropertyName("price")]
    public int Price { get; init; }

    /// <summary>
    /// Ожидаемая сумма следующей покупки у покупателя, связанного с транзакцией
    /// </summary>
    [JsonPropertyName("next_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NextPrice { get; init; }

    /// <summary>
    /// Ожидаемая дата следующей покупки в формате Unix Timestamp у покупателя, связанного с транзакцией
    /// </summary>
    [JsonPropertyName("next_date")]
    public long NextDate { get; init; }

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
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    public AddTransactionRequest(int price)
    {
        Price = price;
    }
}

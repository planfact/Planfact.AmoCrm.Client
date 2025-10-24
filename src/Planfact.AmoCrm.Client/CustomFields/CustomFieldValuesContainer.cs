using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Дополнительное поле сущности amoCRM с заполненными значениями.
/// API возвращает такие модели внутри сущностей (сделки, покупатели, компании, контакты).
/// Отличается от модели дополнительного поля <see cref="CustomField" />
/// </summary>
public sealed record CustomFieldValuesContainer
{
    /// <summary>
    /// Идентификатор поля
    /// </summary>
    [JsonPropertyName("field_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? FieldId { get; init; }

    /// <summary>
    /// Название поля
    /// </summary>
    [JsonPropertyName("field_name")]
    public string FieldName { get; init; } = string.Empty;

    /// <summary>
    /// Символьный код поля, по которому можно обновлять значение в сущности, без передачи Id поля
    /// </summary>
    [JsonPropertyName("field_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FieldCode { get; init; }

    /// <summary>
    /// Массив заполняемых значений
    /// </summary>
    [JsonPropertyName("values")]
    public CustomFieldValue[] Values { get; init; } = [];
}

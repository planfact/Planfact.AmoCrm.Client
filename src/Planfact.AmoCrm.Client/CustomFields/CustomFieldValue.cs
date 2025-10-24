using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Значение дополнительного поля в amoCRM
/// </summary>
public sealed record CustomFieldValue
{
    /// <summary>
    /// Значение поля
    /// </summary>
    [JsonPropertyName("value")]
    [JsonConverter(typeof(CustomFieldValueConverter))]
    public string? Value { get; init; }

    /// <summary>
    /// Символьный код значения поля
    /// </summary>
    [JsonPropertyName("enum_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EnumCode { get; init; }

    /// <summary>
    /// Идентификатор значения поля
    /// </summary>
    [JsonPropertyName("enum_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? EnumId { get; init; }
}

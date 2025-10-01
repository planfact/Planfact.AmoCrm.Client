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
    public dynamic? Value { get; init; }

    /// <summary>
    /// Идентификатор значения поля
    /// </summary>
    [JsonPropertyName("enum_code")]
    public dynamic? EnumCode { get; init; }

    /// <summary>
    /// Символьный код значения поля
    /// </summary>
    [JsonPropertyName("enum_id")]
    public dynamic? EnumId { get; init; }
}

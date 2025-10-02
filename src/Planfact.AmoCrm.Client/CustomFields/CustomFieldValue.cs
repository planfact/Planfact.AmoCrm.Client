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
    public string? Value { get; init; }

    /// <summary>
    /// Символьный код значения поля
    /// </summary>
    [JsonPropertyName("enum_code")]
    public string? EnumCode { get; init; }

    /// <summary>
    /// Идентификатор значения поля
    /// </summary>
    [JsonPropertyName("enum_id")]
    public int? EnumId { get; init; }
}

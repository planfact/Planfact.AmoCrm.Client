using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Режим покупателей в amoCRM
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomersMode
{
    /// <summary>
    /// Функциональность недоступна
    /// </summary>
    [JsonPropertyName("unavailable")]
    Unavailable,

    /// <summary>
    /// Функциональность отключена
    /// </summary>
    [JsonPropertyName("disabled")]
    Disabled,

    /// <summary>
    /// Сегментация покупателей
    /// </summary>
    [JsonPropertyName("segments")]
    Segments,

    /// <summary>
    /// Устаревший режим (deprecated)
    /// </summary>
    [JsonPropertyName("dynamic")]
    Dynamic,

    /// <summary>
    /// Периодические покупки
    /// </summary>
    [JsonPropertyName("periodicity")]
    Periodicity,
}

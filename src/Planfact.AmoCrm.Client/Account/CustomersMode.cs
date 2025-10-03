using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Режим покупателей в amoCRM
/// </summary>
[JsonConverter(typeof(CustomersModeConverter))]
public enum CustomersMode
{
    /// <summary>
    /// Функциональность недоступна
    /// </summary>
    Unavailable,

    /// <summary>
    /// Функциональность отключена
    /// </summary>
    Disabled,

    /// <summary>
    /// Сегментация покупателей
    /// </summary>
    Segments,

    /// <summary>
    /// Устаревший режим (deprecated)
    /// </summary>
    Dynamic,

    /// <summary>
    /// Периодические покупки
    /// </summary>
    Periodicity,
}

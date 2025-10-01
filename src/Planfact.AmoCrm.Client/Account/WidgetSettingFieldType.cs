using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Типы полей настроек виджета amoCRM
/// </summary>
[JsonConverter(typeof(WidgetSettingFieldTypeConverter))]
public enum WidgetSettingFieldType
{
    /// <summary>
    /// Текстовое поле
    /// </summary>
    [JsonPropertyName("text")]
    Text,

    /// <summary>
    /// Поле пароля
    /// </summary>
    [JsonPropertyName("pass")]
    Password,

    /// <summary>
    /// Пользовательское поле
    /// </summary>
    [JsonPropertyName("custom")]
    Custom,

    /// <summary>
    /// Поле пользователей
    /// </summary>
    [JsonPropertyName("users")]
    Users,

    /// <summary>
    /// Поле пользователей с лендинг-пейджем
    /// </summary>
    [JsonPropertyName("users_lp")]
    UsersLandingPage
}

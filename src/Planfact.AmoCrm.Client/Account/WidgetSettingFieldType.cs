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
    Text,

    /// <summary>
    /// Поле пароля
    /// </summary>
    Password,

    /// <summary>
    /// Поле с пользовательской конфигурацией
    /// </summary>
    Custom,

    /// <summary>
    /// Поле пользователей
    /// </summary>
    Users,

    /// <summary>
    /// Поле пользователей с лендингом
    /// </summary>
    UsersLandingPage
}

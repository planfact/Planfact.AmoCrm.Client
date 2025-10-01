using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Шаблон поля настройки виджета
/// </summary>
public sealed record WidgetSettingsTemplate
{
    /// <summary>
    /// Ключ значения поля в настройках виджета
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Название поля в настройках виджета
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Тип поля настройки виджета
    /// </summary>
    [JsonPropertyName("type")]
    public WidgetSettingFieldType Type { get; init; }

    /// <summary>
    /// Является ли настройка обязательной
    /// </summary>
    [JsonPropertyName("is_required")]
    public bool IsRequired { get; init; }
}

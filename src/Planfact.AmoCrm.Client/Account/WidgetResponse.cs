using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Ответ с настройками виджета
/// </summary>
public sealed record WidgetResponse : BaseResponse
{
    /// <summary>
    /// Идентификатор виджета
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Код виджета
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Версия виджета
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Рейтинг виджета
    /// </summary>
    [JsonPropertyName("rating")]
    public float Rating { get; init; }

    /// <summary>
    /// Доступен ли виджет в качестве источника сделок
    /// </summary>
    [JsonPropertyName("is_lead_source")]
    public bool IsLeadSource { get; init; }

    /// <summary>
    /// Доступен ли виджет в Digital Pipeline
    /// </summary>
    [JsonPropertyName("is_work_with_dp")]
    public bool IsDigitalPipelineAvailable { get; init; }

    /// <summary>
    /// Является ли виджет отраслевым CRM-решением
    /// </summary>
    [JsonPropertyName("is_crm_template")]
    public bool IsCrmTemplate { get; init; }

    /// <summary>
    /// UUID связанной с виджетом OAuth-интеграции
    /// </summary>
    [JsonPropertyName("client_uuid")]
    public Guid? ClientUuid { get; init; }

    /// <summary>
    /// Установлен ли виджет в текущем аккаунте
    /// </summary>
    [JsonPropertyName("is_active_in_account")]
    public bool IsActiveInAccount { get; init; }

    /// <summary>
    /// Идентификатор воронки, в которой виджет установлен как источник сделок
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; init; }

    /// <summary>
    /// Шаблоны полей, доступных для настройки виджета
    /// </summary>
    [JsonPropertyName("settings_template")]
    public WidgetSettingsTemplate[] SettingsTemplates { get; init; } = [];

    /// <summary>
    /// Настройки виджета. Возвращается только при запросе с ключом интеграции, в которой установлен виджет
    /// </summary>
    [JsonPropertyName("settings")]
    public string? WidgetSettings { get; init; }
}

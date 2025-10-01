using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Статус сделки в amoCRM
/// </summary>
public sealed record LeadStatus
{
    /// <summary>
    /// ID статуса
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название статуса
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Сортировка статуса
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; init; }

    /// <summary>
    /// Доступен ли статус для редактирования
    /// </summary>
    [JsonPropertyName("is_editable")]
    public bool IsEditable { get; init; }

    /// <summary>
    /// ID воронки, в которой находится статус
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; init; }

    /// <summary>
    /// Цвет статуса в формате HEX
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;

    /// <summary>
    /// Тип статуса
    /// </summary>
    [JsonPropertyName("type")]
    public LeadStatusType Type { get; init; }

    /// <summary>
    /// ID аккаунта, в котором находится статус
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }
}

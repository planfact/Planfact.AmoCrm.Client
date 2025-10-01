using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Модель статуса сделки amoCRM, в рамках которого используется дополнительное поле
/// </summary>
public sealed record CustomFieldLeadStatus
{
    /// <summary>
    /// Идентификатор статуса
    /// </summary>
    [JsonPropertyName("status_id")]
    public int StatusId { get; init; }

    /// <summary>
    /// Идентификатор воронки
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; init; }
}

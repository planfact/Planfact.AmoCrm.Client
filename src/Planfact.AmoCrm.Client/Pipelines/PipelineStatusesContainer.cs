using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Модель статусов воронки
/// </summary>
public sealed record PipelineStatusesContainer
{
    /// <summary>
    /// Данные статусов сделок, доступных в воронке
    /// </summary>
    [JsonPropertyName("statuses")]
    public LeadStatus[] Statuses = [];
}

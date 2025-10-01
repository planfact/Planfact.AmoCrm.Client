using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Результат выполнения задачи в amoCRM
/// </summary>
public sealed record TaskResult
{
    /// <summary>
    /// Текстовое описание результата выполнения задачи
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

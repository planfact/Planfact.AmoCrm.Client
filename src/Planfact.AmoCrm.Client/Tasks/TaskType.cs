using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о типе задач
/// </summary>
public sealed record TaskType
{
    /// <summary>
    /// Идентификатор типа задач
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название типа задач
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Код типа задач
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Цвет типа задач в формате HEX
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; init; }

    /// <summary>
    /// Идентификатор иконки типа задач
    /// </summary>
    [JsonPropertyName("icon_id")]
    public int? IconId { get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Список ошибок валидации запроса
/// </summary>
public sealed record ValidationErrorsCollection
{
    /// <summary>
    /// Id сущности, не прошедшей валидацию
    /// </summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }

    /// <summary>
    /// Массив ошибок валидации конкретной сущности
    /// </summary>
    [JsonPropertyName("errors")]
    public ValidationError[] Errors { get; init; } = [];
}

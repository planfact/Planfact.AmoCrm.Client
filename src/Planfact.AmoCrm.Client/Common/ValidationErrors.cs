using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Ошибки валидации запроса к API amoCRM
/// </summary>
public sealed record ValidationErrors
{
    /// <summary>
    /// Идентификатор запроса, не прошедшего валидацию
    /// </summary>
    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }

    /// <summary>
    /// Ошибки валидации
    /// </summary>
    [JsonPropertyName("errors")]
    public ValidationError[]? Errors { get; init; }
}

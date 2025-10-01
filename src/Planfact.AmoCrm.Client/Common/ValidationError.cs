using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Ошибка валидации
/// </summary>
public sealed record ValidationError
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    /// <summary>
    /// Путь к свойству, не прошедшему валидацию
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path { get; init; }

    /// <summary>
    /// Описание ошибки
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Description { get; init; }
}

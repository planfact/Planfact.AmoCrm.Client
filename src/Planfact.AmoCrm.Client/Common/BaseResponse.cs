using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Базовый ответ от API amoCRM с информацией об ошибках
/// </summary>
public record BaseResponse
{
    /// <summary>
    /// Заголовок ошибки
    /// </summary>
    [JsonPropertyName("title")]
    public string? ErrorTitle { get; init; }

    /// <summary>
    /// Детальное описание ошибки
    /// </summary>
    [JsonPropertyName("detail")]
    public string? ErrorDetails { get; init; }

    /// <summary>
    /// Коллекция ошибок валидации полей
    /// </summary>
    [JsonPropertyName("validation-errors")]
    public ValidationErrorsCollection? ValidationErrors { get; init; }
}

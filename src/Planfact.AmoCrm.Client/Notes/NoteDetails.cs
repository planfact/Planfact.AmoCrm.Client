using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Свойства примечания amoCRM, зависят от типа примечания
/// </summary>
public sealed record NoteDetails
{
    /// <summary>
    /// Текст примечания
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Уникальный идентификатор примечания
    /// </summary>
    [JsonPropertyName("uniq")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Uuid { get; init; }

    /// <summary>
    /// Длительность звонка в секундах. Заполняется только в примечаниях с типом call_in и call_out
    /// </summary>
    [JsonPropertyName("duration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Duration { get; init; }

    /// <summary>
    /// Оператор связи. Заполняется только в примечаниях с типом call_in и call_out
    /// </summary>
    [JsonPropertyName("source")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Source { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за звонок. Заполняется только в примечаниях с типом call_in и call_out
    /// </summary>
    [JsonPropertyName("call_responsible")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CallResponsibleUserId { get; init; }

    /// <summary>
    /// Номер телефона. Заполняется только в примечаниях с типом call_in, call_out, sms_in и sms_out
    /// </summary>
    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; init; }

    /// <summary>
    /// Статус сообщения кассиру. Заполняется только в примечании с типом message_cashier
    /// </summary>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }

    /// <summary>
    /// Адрес. Заполняется только в примечании с типом geolocation
    /// </summary>
    [JsonPropertyName("address")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address { get; init; }

    /// <summary>
    /// Долгота. Заполняется только в примечании с типом geolocation
    /// </summary>
    [JsonPropertyName("longitude")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Longitude { get; init; }

    /// <summary>
    /// Широта. Заполняется только в примечании с типом geolocation
    /// </summary>
    [JsonPropertyName("latitude")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Latitude { get; init; }

    /// <summary>
    /// Версия файла. Заполняется только в примечании с типом attachment
    /// </summary>
    [JsonPropertyName("version_uuid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? FileVersionId { get; init; }

    /// <summary>
    /// Идентификатор файла. Заполняется только в примечании с типом attachment
    /// </summary>
    [JsonPropertyName("file_uuid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? FileId { get; init; }

    /// <summary>
    /// Название файла. Заполняется только в примечании с типом attachment
    /// </summary>
    [JsonPropertyName("file_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FileName { get; init; }
}

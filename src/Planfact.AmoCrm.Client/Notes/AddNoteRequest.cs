using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Модель запроса на создание примечания в API amoCRM
/// </summary>
public sealed record AddNoteRequest
{
    /// <summary>
    ///Идентификатор родительской сущности примечания
    /// </summary>
    [JsonPropertyName("entity_id")]
    public int EntityId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего примечание
    /// </summary>
    [JsonPropertyName("created_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Тип примечания
    /// </summary>
    [JsonPropertyName("note_type")]
    public NoteType NoteType { get; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за примечание
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Нужно ли отправлять события в Digital Pipeline.
    /// Если флаг не передан или передан со значением true, триггеры Digital Pipeline отрабатывать будут, если передано false – не будут.
    /// Влияет на такие триггеры как: счет оплачен, звонок соверешен и другие, которые запускаются при добавлении примечания.
    /// </summary>
    [JsonPropertyName("is_need_to_trigger_digital_pipeline")]
    public bool IsNeedToTriggerDigitalPipeline { get; init; }

    /// <summary>
    /// Свойства примечания
    /// </summary>
    [JsonPropertyName("params")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public NoteDetails? Parameters { get; init; }

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    /// <param name="entityId">Идентификатор сущности, к которой будет привязано примечание</param>
    /// <param name="noteType">Тип примечания</param>
    public AddNoteRequest(int entityId, NoteType noteType)
    {
        EntityId = entityId;
        NoteType = noteType;
    }
}

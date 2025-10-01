using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Ответ API amoCRM, содержащий информацию о примечании
/// </summary>
public sealed record Note
{
    /// <summary>
    /// Идентификатор примечания
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    ///Идентификатор родительской сущности примечания
    /// </summary>
    [JsonPropertyName("entity_id")]
    public int EntityId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, ответственного за примечание
    /// </summary>
    [JsonPropertyName("responsible_user_id")]
    public int? ResponsibleUserId { get; init; }

    /// <summary>
    /// Идентификатор группы, в которой состоит пользователь, ответственный за примечание
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? ResponsibleUserGroupId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создавшего примечание
    /// </summary>
    [JsonPropertyName("created_by")]
    public int? CreatedBy { get; init; }

    /// <summary>
    /// Идентификатор пользователя, редактировавшего примечание последним
    /// </summary>
    [JsonPropertyName("updated_by")]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Дата создания примечания в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// Дата редактирования примечания в формате Unix Timestamp
    /// </summary>
    [JsonPropertyName("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// Тип примечания
    /// <see cref="AmoCrmNoteTypeEnum"/>
    /// </summary>
    [JsonPropertyName("note_type")]
    public string NoteTypeName { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор аккаунта, в котором находится примечание
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Свойства примечания
    /// </summary>
    [JsonPropertyName("params")]
    public NoteDetails? Parameters { get; init; }

    /// <summary>
    /// Получить тип примечания в формате перечисления
    /// </summary>
    public AmoCrmNoteTypeEnum? GetNoteType() => NoteTypeConverter.FromString(NoteTypeName);
}

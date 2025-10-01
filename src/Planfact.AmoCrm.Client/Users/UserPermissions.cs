using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Обобщенная информация о правах пользователя amoCRM
/// </summary>
public sealed record UserPermissions
{
    /// <summary>
    /// Права доступа к сделкам
    /// </summary>
    [JsonPropertyName("leads")]
    public UserEntityPermissions? Leads { get; init; }

    /// <summary>
    /// Права доступа к контактам
    /// </summary>
    [JsonPropertyName("contacts")]
    public UserEntityPermissions? Contacts { get; init; }
    /// <summary>
    /// Права доступа к компаниям
    /// </summary>
    [JsonPropertyName("companies")]
    public UserEntityPermissions? Companies { get; init; }

    /// <summary>
    /// Права доступа к задачам
    /// </summary>
    [JsonPropertyName("tasks")]
    public UserEntityPermissions? Tasks { get; init; }

    /// <summary>
    /// Доступен ли функционал почты
    /// </summary>
    [JsonPropertyName("mail_access")]
    public bool IsMailAccessEnabled { get; init; }

    /// <summary>
    /// Доступен ли функционал списков
    /// </summary>
    [JsonPropertyName("catalog_access")]
    public bool IsCatalogAccessEnabled { get; init; }

    /// <summary>
    /// Права доступа в рамках конкретных статусов сделок
    /// </summary>
    [JsonPropertyName("status_rights")]
    public UserStatusPermissions[]? StatusPermissions { get; init; }

    /// <summary>
    /// Является ли пользователь администратором
    /// </summary>
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; init; }

    /// <summary>
    /// Является ли пользователь бесплатным
    /// </summary>
    [JsonPropertyName("is_free")]
    public bool IsFree { get; init; }

    /// <summary>
    /// Является ли пользователь активным
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }

    /// <summary>
    /// ID группы пользователя
    /// </summary>
    [JsonPropertyName("group_id")]
    public int? GroupId { get; init; }

    /// <summary>
    /// ID роли пользователя
    /// </summary>
    [JsonPropertyName("role_id")]
    public int? RoleId { get; init; }
}

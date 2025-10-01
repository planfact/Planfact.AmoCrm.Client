using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Виды прав доступа пользователей amoCRM
/// </summary>
[JsonConverter(typeof(UserPermissionTypeConverter))]
public enum UserPermissionType
{
    /// <summary>
    /// Полный доступ
    /// </summary>
    [JsonPropertyName("A")]
    All,

    /// <summary>
    /// Доступ в пределах группы
    /// </summary>
    [JsonPropertyName("G")]
    Group,

    /// <summary>
    /// Доступ только к тем объектам, владельцем которых является пользователь
    /// </summary>
    [JsonPropertyName("M")]
    Mine,

    /// <summary>
    /// Доступ запрещен
    /// </summary>
    [JsonPropertyName("D")]
    Denied
}

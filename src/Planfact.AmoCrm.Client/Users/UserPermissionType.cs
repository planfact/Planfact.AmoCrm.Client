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
    All,

    /// <summary>
    /// Доступ в пределах группы
    /// </summary>
    Group,

    /// <summary>
    /// Доступ только к тем объектам, владельцем которых является пользователь
    /// </summary>
    Mine,

    /// <summary>
    /// Доступ запрещен
    /// </summary>
    Denied
}

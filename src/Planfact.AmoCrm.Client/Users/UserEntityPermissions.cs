using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Права пользователя по работе с конкретным типом сущностей (сделки, покупатели и пр.)
/// </summary>
public sealed record UserEntityPermissions
{
    /// <summary>
    /// Право на просмотр
    /// </summary>
    [JsonPropertyName("view")]
    public UserPermissionType View { get; init; }

    /// <summary>
    /// Право на редактирование
    /// </summary>
    [JsonPropertyName("edit")]
    public UserPermissionType Edit { get; init; }

    /// <summary>
    /// Право на добавление
    /// </summary>
    [JsonPropertyName("add")]
    public UserPermissionType Add { get; init; }

    /// <summary>
    /// Право на удаление
    /// </summary>
    [JsonPropertyName("delete")]
    public UserPermissionType Delete { get; init; }

    /// <summary>
    /// Право на экспорт
    /// </summary>
    [JsonPropertyName("export")]
    public UserPermissionType Export { get; init; }
}

using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Вложенные сущности из поля _embedded в запросах к API amoCRM
/// </summary>
public record EmbeddedEntitiesRequest
{
    /// <summary>
    /// Теги
    /// </summary>
    [JsonPropertyName("tags")]
    public Tag[]? Tags { get; init; }

    /// <summary>
    /// Компании
    /// </summary>
    [JsonPropertyName("companies")]
    public Company[]? Companies { get; init; }

    /// <summary>
    /// Контакты
    /// </summary>
    [JsonPropertyName("contacts")]
    public Contact[]? Contacts { get; init; }
}

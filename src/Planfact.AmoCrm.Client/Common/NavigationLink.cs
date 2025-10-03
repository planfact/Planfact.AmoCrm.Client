using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель адреса ресурса в API amoCRM
/// </summary>
public sealed record NavigationLink
{
    /// <summary>
    /// URI ресурса в API amoCRM
    /// </summary>
    [JsonPropertyName("href")]
    public string Uri { get; init; } = string.Empty;
}

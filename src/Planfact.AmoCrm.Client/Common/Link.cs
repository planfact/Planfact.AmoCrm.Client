using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель URI в API amoCRM
/// </summary>
public sealed record Link
{
    /// <summary>
    /// URI сущности в API amoCRM
    /// </summary>
    [JsonPropertyName("href")]
    public string Uri { get; init; } = string.Empty;
}

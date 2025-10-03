using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Модель связанных ссылок для пагинации коллекций в amoCRM
/// </summary>
public sealed record PaginationLinksResponse
{
    /// <summary>
    /// URI текущей страницы коллекции
    /// </summary>
    [JsonPropertyName("self")]
    public NavigationLink? Self { get; init; }

    /// <summary>
    /// URI следующей страницы коллекции
    /// </summary>
    [JsonPropertyName("next")]
    public NavigationLink? Next { get; init; }

    /// <summary>
    /// URI первой страницы коллекции
    /// </summary>
    [JsonPropertyName("first")]
    public NavigationLink? First { get; init; }

    /// <summary>
    /// URI предыдущей страницы коллекции
    /// </summary>
    [JsonPropertyName("prev")]
    public NavigationLink? Prev { get; init; }
}

using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Элемент поля типа <see cref="CustomFieldType.ChainedList"/>
/// </summary>
public sealed record CustomFieldChainedListNode
{
    /// <summary>
    /// Название связанного списка, которое отображается в карточке
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Идентификатор каталога
    /// </summary>
    [JsonPropertyName("catalog_id")]
    public int CatalogId { get; init; }

    /// <summary>
    /// Идентификатор родительского каталога
    /// </summary>
    [JsonPropertyName("parent_catalog_id")]
    public int ParentCatalogId { get; init; }
}

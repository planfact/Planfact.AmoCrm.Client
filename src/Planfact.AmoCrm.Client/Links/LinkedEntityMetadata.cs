using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Модель метаданных связанной сущности
/// </summary>
public sealed record LinkedEntityMetadata
{
    /// <summary>
    /// Идентификатор каталога
    /// </summary>
    [JsonPropertyName("catalog_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CatalogId { get; init; }

    /// <summary>
    /// Количество прикрепленных элементов каталогов
    /// </summary>
    [JsonPropertyName("quantity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? CatalogElementsCount { get; init; }

    /// <summary>
    /// Является ли связанный контакт главным
    /// </summary>
    [JsonPropertyName("is_main")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsMainContact { get; init; }

    /// <summary>
    /// Идентификатор пользователя, выполнившего привязку сущности
    /// </summary>
    [JsonPropertyName("updated_by")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? UpdatedBy { get; init; }

    /// <summary>
    /// Идентификатор дополнительного поля с типом "Цена", которое установлено для связанной сущности
    /// </summary>
    [JsonPropertyName("price_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? PriceCustomFieldId { get; init; }
}

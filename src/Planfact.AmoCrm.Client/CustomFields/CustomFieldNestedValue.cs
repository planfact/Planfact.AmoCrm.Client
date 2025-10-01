using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Модель вложенного значения дополнительного поля amoCRM
/// </summary>
public sealed record CustomFieldNestedValue
{
    /// <summary>
    /// Идентификатор вложенного значения
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Вложенное значение
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор родительского вложенного значения
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; init; }

    /// <summary>
    /// Сортировка вложенного значения
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; init; }
}


using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Доступное значение для дополнительного поля amoCRM
/// </summary>
public sealed record CustomFieldEnumValue
{
    /// <summary>
    /// Идентификатор значения
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Значение поля
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Сортировка значения
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; init; }

    /// <summary>
    /// Символьный код значения
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }
}

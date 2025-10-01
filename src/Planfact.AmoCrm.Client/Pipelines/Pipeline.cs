using System.Text.Json.Serialization;

namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Воронка сделок в amoCRM
/// </summary>
public sealed record Pipeline
{
    /// <summary>
    /// ID воронки
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>
    /// Название воронки
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Сортировка воронки
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; init; }

    /// <summary>
    /// Является ли воронка главной
    /// </summary>
    [JsonPropertyName("is_main")]
    public bool IsMain { get; init; }

    /// <summary>
    /// Включено ли неразобранное в воронке
    /// </summary>
    [JsonPropertyName("is_unsorted_on")]
    public bool IsUnsortedOn { get; init; }

    /// <summary>
    /// Является ли воронка архивной
    /// </summary>
    [JsonPropertyName("is_archive")]
    public bool IsArchive { get; init; }

    /// <summary>
    /// ID аккаунта, в котором находится воронка
    /// </summary>
    [JsonPropertyName("account_id")]
    public int AccountId { get; init; }

    /// <summary>
    /// Допустимые статусы сделок в рамках воронки
    /// </summary>
    [JsonPropertyName("_embedded")]
    public PipelineStatusesContainer? AvailableStatuses { get; init; }
}

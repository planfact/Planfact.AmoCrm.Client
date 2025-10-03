using System.Text.Json.Serialization;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Права пользователя на работу с заданным типом сущностей amoCRM в рамках конкретного статуса сделки
/// </summary>
public sealed record UserStatusPermissions
{
    /// <summary>
    /// Тип сущности (leads)
    /// </summary>
    [JsonPropertyName("entity_type")]
    public string EntityTypeName { get; init; } = string.Empty;

    /// <summary>
    /// ID воронки
    /// </summary>
    [JsonPropertyName("pipeline_id")]
    public int PipelineId { get; init; }

    /// <summary>
    /// ID статуса
    /// </summary>
    [JsonPropertyName("status_id")]
    public int StatusId { get; init; }

    /// <summary>
    /// Права на работу с сущностями
    /// </summary>
    [JsonPropertyName("rights")]
    public UserEntityPermissions? EntityPermissions { get; init; }

    /// <summary>
    /// Получить тип связанной сущности в формате перечисления
    /// </summary>
    public EntityType? GetEntityType() => EntityTypeConverter.FromString(EntityTypeName);
}

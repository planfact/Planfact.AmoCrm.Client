using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Модель фильтра связанных сущностей в amoCRM
/// </summary>
public sealed record EntityLinksFilter
{
    /// <summary>
    /// Идентификаторы главных сущностей
    /// </summary>s
    public IEnumerable<int> EntityIds { get; init; }

    /// <summary>
    /// Идентификатор связанной сущности
    /// </summary>
    public int? LinkedEntityId { get; init; }

    /// <summary>
    /// Тип связанной сущности
    /// </summary>
    public EntityType? LinkedEntityType { get; init; }

    /// <summary>
    /// Идентификатор связанного каталога
    /// </summary>
    public int? CatalogId { get; init; }

    /// <summary>
    /// Конструктор, обеспечивающий инициализацию обязательных полей
    /// </summary>
    /// <param name="entityIds">Идентификаторы главных сущностей</param>
    public EntityLinksFilter(IEnumerable<int> entityIds)
    {
        if (!entityIds.Any())
        {
            throw new ArgumentException("Должен быть задан хотя бы один идентификатор", nameof(entityIds));
        }

        EntityIds = entityIds;
    }
}

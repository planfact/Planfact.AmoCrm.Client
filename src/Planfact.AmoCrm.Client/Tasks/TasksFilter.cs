using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Модель фильтра задач в amoCRM
/// </summary>
public sealed record TasksFilter
{
    /// <summary>
    /// Идентификаторы задач
    /// </summary>s
    public IEnumerable<int> TaskIds { get; init; } = [];

    /// <summary>
    /// Идентификаторы типов задач
    /// </summary>s
    public IEnumerable<int> TaskTypeIds { get; init; } = [];

    /// <summary>
    /// Идентификаторы сущностей, привязанных к задаче
    /// </summary>s
    public IEnumerable<int> EntityIds { get; init; } = [];

    /// <summary>
    /// Тип сущности, привязанной к задаче
    /// </summary>
    public EntityType? EntityType { get; init; }

    /// <summary>
    /// Идентификаторы пользователей, ответственных за задачи
    /// </summary>
    public IEnumerable<int> ResponsibleUserIds { get; init; } = [];

    /// <summary>
    /// Начальное значение фильтра по диапазону дат обновления задач в формате Unix Timestamp
    /// </summary>
    public long? UpdatedAtFrom { get; init; }

    /// <summary>
    /// Конечное значение фильтра по диапазону дат обновления задач в формате Unix Timestamp
    /// </summary>
    public long? UpdatedAtTo { get; init; }

    /// <summary>
    /// Признак завершения задачи
    /// </summary>
    public bool? IsCompleted { get; init; }
}

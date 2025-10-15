using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Контракт сервиса связей сущностей в amoCRM
/// </summary>
public interface IAmoCrmLinkService
{
    /// <summary>
    /// Получение связей сущности по заданному фильтру
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, связи которого необходимо получить</param>
    /// <param name="filter">Фильтр связанных сущностей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task<IReadOnlyCollection<EntityLink>> GetLinksAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        EntityLinksFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Привязка сущностей с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, к которой выполняется привязка</param>
    /// <param name="requests">Коллекция запросов на создание связей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task<IReadOnlyCollection<EntityLink>> LinkAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<LinkEntitiesRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отвязка сущностей с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, от которой выполняется отвязка</param>
    /// <param name="requests">Коллекция запросов на удаление связей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task UnlinkAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<UnlinkEntitiesRequest> requests,
        CancellationToken cancellationToken = default);
}

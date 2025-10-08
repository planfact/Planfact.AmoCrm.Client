using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Leads;

/// <summary>
/// Контракт сервиса сделок в amoCRM
/// </summary>
public interface IAmoCrmLeadService
{
    /// <summary>
    /// Получение списка сделок из аккаунта в amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка сделок из аккаунта amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение сделок из аккаунта в amoCRM по заданным ID
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="ids">Идентификаторы сделок, которые необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение сделок из аккаунта amoCRM по заданным ID с поддержкой включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="ids">Идентификаторы сделок, которые необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание сделок в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание сделок</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных сделках</returns>
    public Task<IReadOnlyCollection<Lead>> AddLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddLeadRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование сделок в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование сделок</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных сделках</returns>
    public Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default);
}

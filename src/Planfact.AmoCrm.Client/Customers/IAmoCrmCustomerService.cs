using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Customers;

/// <summary>
/// Контракт сервиса покупателей в amoCRM
/// </summary>
public interface IAmoCrmCustomerService
{
    /// <summary>
    /// Получение списка покупателей из аккаунта в amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных покупателях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка покупателей из аккаунта amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных покупателях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание покупателей в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание покупателей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных покупателях</returns>
    public Task<IReadOnlyCollection<Customer>> AddCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование покупателей в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование покупателей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных покупателях</returns>
    public Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default);
}

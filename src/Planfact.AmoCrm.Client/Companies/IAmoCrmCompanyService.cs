using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Companies;

/// <summary>
/// Контракт сервиса компаний в amoCRM
/// </summary>
public interface IAmoCrmCompanyService
{
    /// <summary>
    /// Получение списка компаний из аккаунта в amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных компаниях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка компаний из аккаунта amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных компаниях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание компаний в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание компаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных компаниях</returns>
    public Task<IReadOnlyCollection<Company>> AddCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование компаний в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование компаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных компаниях</returns>
    public Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default);
}

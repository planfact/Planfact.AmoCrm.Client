
using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Contacts;

/// <summary>
/// Контракт сервиса контактов в amoCRM
/// </summary>
public interface IAmoCrmContactService
{
    /// <summary>
    /// Получение списка контактов из аккаунта amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список контактов. Если ничего не найдено, возвращает пустой список</returns>
    public Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка контактов из аккаунта amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список контактов. Если ничего не найдено, возвращает пустой список</returns>
    public Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение контакта из аккаунта amoCRM по идентификатору
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="contactId">Идентификатор контакта</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий информацию о контакте в amoCRM</returns>
    public Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение контакта из аккаунта amoCRM по идентификатору с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="contactId">Идентификатор контакта</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий информацию о контакте в amoCRM</returns>
    public Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание контактов в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание контактов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных контактов</returns>
    public Task<IReadOnlyCollection<Contact>> AddContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование контактов в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование контактов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список обновленных контактов</returns>
    public Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default);
}

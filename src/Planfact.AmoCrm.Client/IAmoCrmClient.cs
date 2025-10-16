using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Links;
using Planfact.AmoCrm.Client.Notes;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.Users;

using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;
using Task = System.Threading.Tasks.Task;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Контракт клиента для работы с API amoCRM v4
/// </summary>
public interface IAmoCrmClient
{
    /// <summary>
    /// Получение авторизационных данных в рамках пользовательской интеграции с amoCRM
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM, к которому привязана пользовательская интеграция</param>
    /// <param name="authorizationCode">Код авторизации, предназначенный для обмена на токены доступа</param>
    /// <param name="redirectUri">URI, на который должны перенаправляться входящие запросы из amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий авторизационные данные для пользовательской интеграции</returns>
    public Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение авторизационных данных в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий авторизационные данные для серверной интеграции</returns>
    public Task<AuthorizationTokens> AuthorizeInternalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление авторизационных данных в рамках пользовательской интеграции с amoCRM
    /// </summary>
    /// <param name="subdomain">Поддомен amoCRM, к которому привязана пользовательская интеграция</param>
    /// <param name="refreshToken">Токен обновления авторизационных данных по протоколу OAuth 2.0</param>
    /// <param name="redirectUri">URI, на который должны перенаправляться входящие запросы из amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий авторизационные данные для пользовательской интеграции</returns>
    public Task<AuthorizationTokens> RefreshTokenAsync(
        string subdomain,
        string refreshToken,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновление авторизационных данных в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="refreshToken">Токен обновления авторизационных данных по протоколу OAuth 2.0</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий авторизационные данные для серверной интеграции</returns>
    public Task<AuthorizationTokens> RefreshTokenInternalAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

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
    /// Получение списка сделок в рамках серверной интеграции с amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка сделок в рамках серверной интеграции с amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
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
    /// Получение сделок в рамках серверной интеграции с amoCRM по заданным ID
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="ids">Идентификаторы сделок, которые необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение сделок в рамках серверной интеграции с amoCRM по заданным ID с поддержкой включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="ids">Идентификаторы сделок, которые необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных сделках. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Lead>> GetLeadsInternalAsync(
        string accessToken,
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
    /// Создание сделок в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание сделок</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных сделках</returns>
    public Task<IReadOnlyCollection<Lead>> AddLeadsInternalAsync(
        string accessToken,
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

    /// <summary>
    /// Редактирование сделок в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование сделок</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных сделках</returns>
    public Task<IReadOnlyCollection<Lead>> UpdateLeadsInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default);

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
    /// Получение списка компаний в рамках серверной интеграции с amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных компаниях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Company>> GetCompaniesInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка компаний в рамках серверной интеграции с amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных компаниях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Company>> GetCompaniesInternalAsync(
        string accessToken,
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
    /// Создание компаний в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание компаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных компаниях</returns>
    public Task<IReadOnlyCollection<Company>> AddCompaniesInternalAsync(
        string accessToken,
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

    /// <summary>
    /// Редактирование компаний в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование компаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных компаниях</returns>
    public Task<IReadOnlyCollection<Company>> UpdateCompaniesInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка задач из аккаунта в amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных компаниях</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка задач в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных компаниях</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> GetTasksInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка задач из аккаунта в amoCRM по заданному фильтру
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="filter">Фильтр задач amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных компаниях</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        TasksFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка задач в рамках серверной интеграции с amoCRM по заданному фильтру
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="filter">Фильтр задач amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных компаниях</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> GetTasksInternalAsync(
        string accessToken,
        TasksFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание задач в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание задач в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> AddTasksInternalAsync(
        string accessToken,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование задач в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование задач в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default);

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
    /// Получение списка покупателей в рамках серверной интеграции с amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных покупателях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Customer>> GetCustomersInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка покупателей в рамках серверной интеграции с amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о найденных покупателях. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<Customer>> GetCustomersInternalAsync(
        string accessToken,
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
    /// Создание покупателей в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание покупателей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных покупателях</returns>
    public Task<IReadOnlyCollection<Customer>> AddCustomersInternalAsync(
        string accessToken,
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

    /// <summary>
    /// Редактирование покупателей в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование покупателей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных покупателях</returns>
    public Task<IReadOnlyCollection<Customer>> UpdateCustomersInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение метаданных аккаунта amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий параметры аккаунта в amoCRM</returns>
    public Task<AccountResponse> GetAccountAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение метаданных аккаунта серверной интеграции amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий параметры аккаунта в amoCRM</returns>
    public Task<AccountResponse> GetAccountInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка пользователей, привязанных к учетной записи amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о пользователях аккаунта. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<User>> GetUsersAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка пользователей в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о пользователях аккаунта. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<User>> GetUsersInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение виджета по коду
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="widgetCode">Код виджета, заданный при загрузке виджета в amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий параметры виджета в amoCRM</returns>
    public Task<WidgetResponse> GetWidgetAsync(
        string accessToken,
        string subdomain,
        string widgetCode,
        CancellationToken cancellationToken = default);

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
    /// Получение списка контактов в рамках серверной интеграции с amoCRM с поддержкой поиска
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список контактов. Если ничего не найдено, возвращает пустой список</returns>
    public Task<IReadOnlyCollection<Contact>> GetContactsInternalAsync(
        string accessToken,
        string query = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка контактов в рамках серверной интеграции с amoCRM с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="query">Поисковый запрос</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список контактов. Если ничего не найдено, возвращает пустой список</returns>
    public Task<IReadOnlyCollection<Contact>> GetContactsInternalAsync(
        string accessToken,
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
    /// Получение контакта в рамках серверной интеграции с amoCRM по идентификатору
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="contactId">Идентификатор контакта</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий информацию о контакте в amoCRM</returns>
    public Task<Contact> GetContactByIdInternalAsync(
        string accessToken,
        int contactId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение контакта в рамках серверной интеграции с amoCRM по идентификатору с поддержкой поиска и включения связанных сущностей
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="contactId">Идентификатор контакта</param>
    /// <param name="linkedEntityTypes">Типы связанных сущностей, информацию о которых необходимо включить в ответ API</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Объект, содержащий информацию о контакте в amoCRM</returns>
    public Task<Contact> GetContactByIdInternalAsync(
        string accessToken,
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
    /// Создание контактов в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание контактов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных контактов</returns>
    public Task<IReadOnlyCollection<Contact>> AddContactsInternalAsync(
        string accessToken,
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

    /// <summary>
    /// Редактирование контактов в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование контактов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список обновленных контактов</returns>
    public Task<IReadOnlyCollection<Contact>> UpdateContactsInternalAsync(
        string accessToken,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка транзакций покупателя в amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список транзакций. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение списка транзакций покупателя в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список транзакций. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Transaction>> GetTransactionsInternalAsync(
        string accessToken,
        int customerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание транзакций, привязанных к покупателю в amoCRM, с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="requests">Коллекция запросов на добавление транзакций</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных транзакций</returns>
    public Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание транзакций, привязанных к покупателю, в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="requests">Коллекция запросов на добавление транзакций</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных транзакций</returns>
    public Task<IReadOnlyCollection<Transaction>> AddTransactionsInternalAsync(
        string accessToken,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение дополнительных полей, настроенных в аккаунте amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности в amoCRM, поля которой необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список дополнительных полей. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<CustomField>> GetCustomFieldsAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение дополнительных полей в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности в amoCRM, поля которой необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список дополнительных полей. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<CustomField>> GetCustomFieldsInternalAsync(
        string accessToken,
        EntityType entityType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение доступных статусов сделок в аккаунте amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список статусов сделок. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение доступных статусов сделок в рамках серверной интеграции с amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список статусов сделок. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesInternalAsync(
        string accessToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение примечаний из аккаунта amoCRM по заданным фильтрам
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="noteType">Тип примечания amoCRM</param>
    /// <param name="entityId">Идентификатор сущности amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список примечаний. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Note>> GetNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        NoteType noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение примечаний в рамках серверной интеграции с amoCRM по заданным фильтрам
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="noteType">Тип примечания amoCRM</param>
    /// <param name="entityId">Идентификатор сущности amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список примечаний. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Note>> GetNotesInternalAsync(
        string accessToken,
        EntityType entityType,
        NoteType noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание примечаний в аккаунте amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="requests">Коллекция запросов на добавление примечаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных примечаний</returns>
    public Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание примечаний в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="requests">Коллекция запросов на добавление примечаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных примечаний</returns>
    public Task<IReadOnlyCollection<Note>> AddNotesInternalAsync(
        string accessToken,
        EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение связей сущности из аккаунта amoCRM по заданному фильтру
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
    /// Получение связей сущности в рамках серверной интеграции с amoCRM по заданному фильтру
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, связи которого необходимо получить</param>
    /// <param name="filter">Фильтр связанных сущностей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task<IReadOnlyCollection<EntityLink>> GetLinksInternalAsync(
        string accessToken,
        EntityType entityType,
        EntityLinksFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Привязка сущностей в amoCRM с поддержкой пакетной обработки
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
    /// Привязка сущностей в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, к которой выполняется привязка</param>
    /// <param name="requests">Коллекция запросов на создание связей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task<IReadOnlyCollection<EntityLink>> LinkInternalAsync(
        string accessToken,
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

    /// <summary>
    /// Отвязка сущностей в рамках серверной интеграции с amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM, от которой выполняется отвязка</param>
    /// <param name="requests">Коллекция запросов на удаление связей</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    public Task UnlinkInternalAsync(
        string accessToken,
        EntityType entityType,
        IReadOnlyCollection<UnlinkEntitiesRequest> requests,
        CancellationToken cancellationToken = default);
}

using Reliable.HttpClient;

namespace Planfact.AmoCrm.Client;

/// <summary>
/// Настройки клиента API amoCRM
/// </summary>
public sealed class AmoCrmClientOptions : HttpClientOptions
{
    /// <summary>
    /// Идентификатор клиентского приложения для OAuth авторизации
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Секретный ключ клиентского приложения для OAuth авторизации
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Код виджета для интеграции через виджеты amoCRM
    /// </summary>
    public string WidgetCode { get; set; } = string.Empty;

    /// <summary>
    /// Название поля виджета для хранения API-ключа
    /// </summary>
    public string WidgetApiKeyFieldName { get; set; } = string.Empty;

    /// <summary>
    /// Путь для OAuth callback от виджетов
    /// </summary>
    public string WidgetCallbackPath { get; set; } = "/api/v1/amocrmIntegrationWidget/OAuthWebhook";

    /// <summary>
    /// URI для OAuth redirect при серверной интеграции
    /// </summary>
    public string ServerIntegrationRedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Код авторизации для серверной интеграции
    /// </summary>
    public string ServerIntegrationAuthCode { get; set; } = string.Empty;

    /// <summary>
    /// Поддомен amoCRM для серверной интеграции
    /// </summary>
    public string ServerIntegrationSubdomain { get; set; } = string.Empty;

    /// <summary>
    /// Путь к OAuth token endpoint
    /// </summary>
    public string OAuthTokenPath { get; set; } = "oauth2/access_token";

    /// <summary>
    /// Базовый путь для API v4 endpoints
    /// </summary>
    public string BaseApiPath { get; set; } = "api/v4";

    /// <summary>
    /// Путь к API сделок
    /// </summary>
    public string LeadsApiPath { get; set; } = "api/v4/leads";

    /// <summary>
    /// Путь к API покупателей
    /// </summary>
    public string CustomersApiPath { get; set; } = "api/v4/customers";

    /// <summary>
    /// Путь к API аккаунта
    /// </summary>
    public string AccountsApiPath { get; set; } = "api/v4/account";

    /// <summary>
    /// Путь к API задач
    /// </summary>
    public string TasksApiPath { get; set; } = "api/v4/tasks";

    /// <summary>
    /// Путь к API компаний
    /// </summary>
    public string CompaniesApiPath { get; set; } = "api/v4/companies";

    /// <summary>
    /// Путь к API виджетов
    /// </summary>
    public string WidgetsApiPath { get; set; } = "api/v4/widgets";

    /// <summary>
    /// Путь к API пользователей
    /// </summary>
    public string UsersApiPath { get; set; } = "api/v4/users";

    /// <summary>
    /// Путь к API контактов
    /// </summary>
    public string ContactsApiPath { get; set; } = "api/v4/contacts";

    /// <summary>
    /// Путь к API транзакций
    /// </summary>
    public string TransactionsApiPath { get; set; } = "api/v4/customers/transactions";

    /// <summary>
    /// Путь к API воронок
    /// </summary>
    public string PipelinesApiPath { get; set; } = "api/v4/leads/pipelines";

    /// <summary>
    /// Название ресурса дополнительных полей в API amoCRM.
    /// Полный путь формируется динамически в зависимости от типа связанной сущности
    /// </summary>
    public string CustomFieldsApiResourceName { get; set; } = "custom_fields";

    /// <summary>
    /// Название ресурса транзакций в API amoCRM.
    /// Полный путь формируется динамически в зависимости от Id покупателя
    /// </summary>
    public string TransactionsApiResourceName { get; set; } = "transactions";

    /// <summary>
    /// Название ресурса примечаний в API amoCRM.
    /// Полный путь формируется динамически в зависимости от типа связанной сущности
    /// </summary>
    public string NotesApiResourceName { get; set; } = "notes";

    /// <summary>
    /// Название ресурса связей сущностей в API amoCRM.
    /// Полный путь формируется динамически в зависимости от типа связанной сущности
    /// </summary>
    public string LinksApiResourceName { get; set; } = "links";

    /// <summary>
    /// Название конечной точки создания связей сущностей в API amoCRM.
    /// Полный путь формируется динамически в зависимости от типа связанной сущности
    /// </summary>
    public string CreateLinksActionName { get; set; } = "link";

    /// <summary>
    /// Название конечной точки удаления связей сущностей в API amoCRM.
    /// Полный путь формируется динамически в зависимости от типа связанной сущности
    /// </summary>
    public string DeleteLinksActionName { get; set; } = "unlink";

    /// <summary>
    /// Время жизни кэша в минутах. Используется только в <see cref="CachedAmoCrmClient"/>
    /// </summary>
    public int CacheExpiryMinutes { get; set; } = 10;

    /// <summary>
    /// Максимальный размер кэша в записях. Используется только в <see cref="CachedAmoCrmClient"/>
    /// </summary>
    public int MaxCacheSize { get; set; } = 1000;

    /// <summary>
    /// Создает экземпляр с настройками по умолчанию
    /// </summary>
    public AmoCrmClientOptions()
    {
        UserAgent = "AmoCrm-DotNet-Client/1.0";
        TimeoutSeconds = 120;
    }
}

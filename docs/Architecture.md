# Архитектура

Описание архитектуры и дизайна библиотеки Planfact.AmoCrm.Client.

## Обзор

Библиотека спроектирована с учетом принципов SOLID и enterprise требований:

- **Типобезопасность**: полная поддержка nullable reference types
- **Асинхронность**: async/await паттерны везде
- **Dependency Injection**: готовая интеграция с `Microsoft.Extensions.DependencyInjection`
- **Логирование**: встроенная поддержка `Microsoft.Extensions.Logging`
- **Тестируемость**: все компоненты легко мокаются

## Структура проекта

Логика разделена по бизнес-доменам, каждый из которых представляет отдельную сущность amoCRM (сделки, покупатели и пр.)

```shell
├── Account/                        # Работа с аккаунтом
│   ├── AmoCrmAccountService.cs
│   └── IAmoCrmAccountService.cs
├── Authorization/                  # Авторизация
│   ├── AmoCrmAuthorizationService.cs
│   └── IAmoCrmAuthorizationService.cs
├── Companies/                      # Компании
│   ├── AmoCrmCompanyService.cs
│   └── IAmoCrmCompanyService.cs
├── Contacts/                       # Контакты
│   ├── AmoCrmContactService.cs
│   └── IAmoCrmContactService.cs
├── CustomFields/                   # Пользовательские поля
│   ├── AmoCrmCustomFieldService.cs
│   └── IAmoCrmCustomFieldService.cs
├── Customers/                      # Клиенты
│   ├── AmoCrmCustomerService.cs
│   └── IAmoCrmCustomerService.cs
├── Leads/                          # Сделки
│   ├── AmoCrmLeadService.cs
│   └── IAmoCrmLeadService.cs
├── Notes/                          # Примечания
│   ├── AmoCrmNoteService.cs
│   └── IAmoCrmNoteService.cs
├── Pipelines/                      # Воронки
│   ├── AmoCrmPipelineService.cs
│   └── IAmoCrmPipelineService.cs
├── Tasks/                          # Задачи
│   ├── AmoCrmTaskService.cs
│   └── IAmoCrmTaskService.cs
├── Transactions/                   # Транзакции
│   ├── AmoCrmTransactionService.cs
│   └── IAmoCrmTransactionService.cs
├── Users/                          # Пользователи
│   ├── AmoCrmUserService.cs
│   └── IAmoCrmUserService.cs
├── Common/                         # Общие компоненты
│   ├── AmoCrmServiceBase.cs
│   └── EntityTypeEnum.cs
├── Exceptions/                     # Обработка ошибок
├── AmoCrmClientOptions.cs          # Настройки клиента
├── AmoCrmHttpResponseHandler.cs    # Обработчик ответов HTTP
├── AmoCrmUriBuilderFactory.cs      # Вспомогательный класс для построения URI
├── IAmoCrmClient.cs                # Основной интерфейс
├── AmoCrmClient.cs                 # Базовая реализация клиента
└── CachedAmoCrmClient.cs           # Реализация с кэшированием
```

## Основные компоненты

### IAmoCrmClient

Главный интерфейс для интеграции с API amoCRM:

```csharp
public interface IAmoCrmClient
{
    Task<AuthorizationTokens> AuthorizeAsync(string subdomain, string authorizationCode, string redirectUri, CancellationToken cancellationToken = default);
    Task<AuthorizationTokens> RefreshTokenAsync(string subdomain, string refreshToken, string redirectUri, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lead>> GetLeadsAsync(string accessToken, string subdomain, string query = "", CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lead>> GetLeadsAsync(string accessToken, string subdomain, IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lead>> AddLeadsAsync(string accessToken, string subdomain, IReadOnlyCollection<AddLeadRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(string accessToken, string subdomain, IReadOnlyCollection<UpdateLeadRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Company>> GetCompaniesAsync(string accessToken, string subdomain, string query = "", CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Company>> AddCompaniesAsync(string accessToken, string subdomain, IReadOnlyCollection<AddCompanyRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(string accessToken, string subdomain, IReadOnlyCollection<UpdateCompanyRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(string accessToken, string subdomain, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(string accessToken, string subdomain, IReadOnlyCollection<AddTaskRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(string accessToken, string subdomain, IReadOnlyCollection<UpdateTaskRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Customer>> GetCustomersAsync(string accessToken, string subdomain, string query = "", CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Customer>> AddCustomersAsync(string accessToken, string subdomain, IReadOnlyCollection<AddCustomerRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(string accessToken, string subdomain, IReadOnlyCollection<UpdateCustomerRequest> requests, CancellationToken cancellationToken = default);
    Task<AmoCrm.Client.Account.AccountResponse> GetAccountAsync(string accessToken, string subdomain, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetUsersAsync(string accessToken, string subdomain, CancellationToken cancellationToken = default);
    Task<WidgetResponse> GetWidgetAsync(string accessToken, string subdomain, string widgetCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Contact>> GetContactsAsync(string accessToken, string subdomain, string query = "", CancellationToken cancellationToken = default);
    Task<Contact> GetContactByIdAsync(string accessToken, string subdomain, int contactId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Contact>> AddContactsAsync(string accessToken, string subdomain, IReadOnlyCollection<AddContactRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(string accessToken, string subdomain, IReadOnlyCollection<UpdateContactRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(string accessToken, string subdomain, int customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(string accessToken, string subdomain, int customerId, IReadOnlyCollection<AddTransactionRequest> requests, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomField>> GetCustomFieldsAsync(string accessToken, string subdomain, AmoCrm.Client.Common.EntityTypeEnum entityType, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesAsync(string accessToken, string subdomain, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Note>> GetNotesAsync(string accessToken, string subdomain, AmoCrm.Client.Common.EntityTypeEnum entityType, AmoCrmNoteTypeEnum noteType, int? entityId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Note>> AddNotesAsync(string accessToken, string subdomain, AmoCrm.Client.Common.EntityTypeEnum entityType, IReadOnlyCollection<AddNoteRequest> requests, CancellationToken cancellationToken = default);

    // Перегрузки для серверных интеграций
}
```

### Сервисы для работы с бизнес-сущностями

```csharp
public interface IAmoCrmAccountService { //... }
public interface IAmoCrmAuthorizationService { //... }
public interface IAmoCrmLeadService { //... }
public interface IAmoCrmCompanyService { //... }
public interface IAmoCrmTaskService { //... }
public interface IAmoCrmCustomerService { //... }
public interface IAmoCrmUserService { //... }
public interface IAmoCrmContactService { //... }
public interface IAmoCrmTransactionService { //... }
public interface IAmoCrmCustomFieldService { //... }
public interface IAmoCrmPipelineService { //... }
public interface IAmoCrmNoteService { //... }
```

### AmoCrmServiceBase

Базовый класс, инкапсулирующий общие механизмы взаимодействия с API amoCRM:

- пагинация
- пакетная отправка запросов
- установка стандартных заголовков в запросах
- валидация ключей доступа
- контроль соблюдения условий использования API

```csharp
public abstract class AmoCrmServiceBase(IHttpClientAdapter httpClient, ILogger logger)
{
    protected const int MaxEntitiesPerBatch = 50;
    protected const int PaginationStartPage = 1;
    protected const int PaginationPerPageLimit = 250;

    protected readonly IHttpClientAdapter HttpClient = httpClient;
    protected readonly ILogger Logger = logger;

    private protected static IDictionary<string, string> GetDefaultHeaders(string accessToken)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Authorization", $"Bearer {accessToken}" },
        };
    }

    private protected IAsyncEnumerable<TResponse> PatchInBatchesAsync<TRequest, TResponse>(
        Uri uri,
        string accessToken,
        IReadOnlyCollection<TRequest> payload,
        CancellationToken cancellationToken = default) where TResponse : class
    {
        return SendInBatchesAsync(
            uri,
            accessToken,
            payload,
            HttpClient.PatchAsync<TRequest[], TResponse>,
            cancellationToken
        );
    }

    private protected IAsyncEnumerable<TResponse> PostInBatchesAsync<TRequest, TResponse>(
        Uri uri,
        string accessToken,
        IReadOnlyCollection<TRequest> payload,
        CancellationToken cancellationToken = default) where TResponse : class
    {
        return SendInBatchesAsync(
            uri,
            accessToken,
            payload,
            HttpClient.PostAsync<TRequest[], TResponse>,
            cancellationToken
        );
    }

    // ...
}

```

### AmoCrmClientOptions

Конфигурация клиента:

```csharp
public sealed class AmoCrmClientOptions : HttpClientOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string WidgetCode { get; set; } = string.Empty;
    public string WidgetApiKeyFieldName { get; set; } = string.Empty;
    public string WidgetCallbackPath { get; set; } = "/api/v1/amocrmIntegrationWidget/OAuthWebhook";
    public string ServerIntegrationRedirectUri { get; set; } = string.Empty;
    public string ServerIntegrationAuthCode { get; set; } = string.Empty;
    public string ServerIntegrationSubdomain { get; set; } = string.Empty;
    public string OAuthTokenPath { get; set; } = "oauth2/access_token";
    public string BaseApiPath { get; set; } = "api/v4";
    public string LeadsApiPath { get; set; } = "api/v4/leads";
    public string CustomersApiPath { get; set; } = "api/v4/customers";
    public string AccountsApiPath { get; set; } = "api/v4/account";
    public string TasksApiPath { get; set; } = "api/v4/tasks";
    public string CompaniesApiPath { get; set; } = "api/v4/companies";
    public string WidgetsApiPath { get; set; } = "api/v4/widgets";
    public string UsersApiPath { get; set; } = "api/v4/users";
    public string ContactsApiPath { get; set; } = "api/v4/contacts";
    public string TransactionsApiPath { get; set; } = "api/v4/customers/transactions";
    public string PipelinesApiPath { get; set; } = "api/v4/leads/pipelines";
    public string CustomFieldsApiResourceName { get; set; } = "custom_fields";
    public string TransactionsApiResourceName { get; set; } = "transactions";
    public string NotesApiResourceName { get; set; } = "notes";
    public int CacheExpiryMinutes { get; set; } = 10;
    public int MaxCacheSize { get; set; } = 1000;
}
```

## Типы интеграции

### Серверная

Для доступа к API используются параметры из настроек:

```csharp
public virtual async Task<AuthorizationTokens> AuthorizeInternalAsync(CancellationToken cancellationToken = default)
{
    return await _authorizationService.AuthorizeAsync(
        _options.ServerIntegrationSubdomain,
        _options.ServerIntegrationAuthCode,
        _options.ServerIntegrationRedirectUri,
        cancellationToken
    ).ConfigureAwait(false);
}
```

### Клиентская

Данные, необходимые для интеграции поступают из amoCRM через webhook, URL для обработки которого задается в свойстве `WidgetCallbackPath` настроек клиента

```csharp
public virtual async Task<AuthorizationTokens> AuthorizeAsync(
        string subdomain,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
{
    return await _authorizationService.AuthorizeAsync(
        subdomain,
        authorizationCode,
        redirectUri,
        cancellationToken
    ).ConfigureAwait(false);
}
```

## Dependency Injection

### Регистрация клиента

```csharp
// Базовая реализация с политиками отказоустойчивости
builder.Services.AddAmoCrmClient(builder.Configuration);

// Реализация с поддержкой кэширования
builder.Services.AddCachedAmoCrmClient(builder.Configuration);
```

### HttpClient management и Resilience

Библиотека использует **Reliable.HttpClient** для enterprise-готовности:

- **Автоматические retry**: повторные попытки при временных сбоях
- **Circuit Breaker**: защита от каскадных отказов
- **Timeout policies**: настраиваемые таймауты
- **Exponential backoff**: умная стратегия повторов
- **HttpClient pooling**: эффективное управление соединениями через `IHttpClientFactory`

```csharp
// Автоматическая настройка
services.AddAmoCrmClient(configuration); // Использует AddResilience() по умолчанию
```

## Обработка ошибок

### Типы ошибок

В данном проекте применяются как системные, так и специальные типизированные исключения для обработки ошибок

1. **ArgumentException** – некорректные параметры (eager validation)
3. **TaskCanceledException** – таймауты
4. **JsonException** - ошибки парсинга ответа
5. **InvalidOperationException** – ошибки конфигурации
6. **AmoCrmHttpException** – сетевые ошибки
7. **AmoCrmValidationException** – ошибки валидации запросов на стороне amoCRM
8. **AmoCrmAuthenticationException** – ошибки получения доступа (неверные учетные данные, истечение срока подписки, блокировка по IP)

## Логирование

### Уровни логирования

- **Debug**: детали HTTP запросов и ответов
- **Information**: успешные запросы
- **Warning**: ошибки обработки ответов API
- **Error**: сетевые ошибки, таймауты, необработанные исключения

### Structured logging

```csharp
_logger.LogDebug("Загрузка сделок из аккаунта {Subdomain}. Поиск по вхождению {Query}", subdomain, query);
_logger.LogWarning(ex, "Не удалось прочитать содержимое HTTP ответа");
```

## Производительность и надежность

### Resilience Patterns

Библиотека построена на основе **Reliable.HttpClient**, который предоставляет enterprise-уровень надежности:

#### Retry Policy

- **Exponential backoff**: задержки между повторами увеличиваются экспоненциально
- **Jitter**: случайные отклонения предотвращают thundering herd
- **Selective retries**: только для retriable ошибок (сетевые, 5xx коды)

#### Circuit Breaker

- **Fail-fast**: быстрое переключение при множественных сбоях
- **Half-open state**: автоматическое восстановление
- **Защита downstream**: предотвращение каскадных отказов

#### Timeout Policies

- **Request timeout**: таймаут на уровне запроса
- **Connection timeout**: таймаут установки соединения
- **Overall timeout**: общий таймаут операции

### Рекомендации по производительности

1. **HttpClient pooling**: используйте DI registration для автоматического pooling
2. **Таймауты**: не используйте слишком низкие значения timeout (10-30 сек), поскольку при обработке batch-запросов время отклика amoCRM может превышать 60 сек
4. **Graceful degradation**: обрабатывайте недоступность сервиса
5. **Monitoring**: используйте логирование для мониторинга retry/circuit breaker событий

### Метрики

Рекомендуется отслеживать:

- Время отклика API
- Количество успешных/неуспешных запросов
- Количество сетевых ошибок

## Тестирование

### Unit тесты

```csharp
// Мокирование интерфейса
var mockClient = new Mock<IAmoCrmClient>();
mockClient.Setup(x => x.GetAccountAsync(It.IsAny<string>(), It.IsAny<string>(), default))
            .ReturnsAsync(new AccountResponse());
```

### Test doubles

Для разных сценариев тестирования библиотека предоставляет легкое мокирование через DI.

## Примеры использования

### Авторизация

```csharp
// Авторизация через OAuth 2.0
var tokens = await _amoCrmClient.Authorization.AuthorizeAsync(
    subdomain: "example",
    authCode: "auth_code",
    redirectUri: "https://app.example.com"
);

// Обновление токенов доступа
var newTokens = await _amoCrmClient.RefreshTokenAsync(
    subdomain: "example",
    refreshToken: tokens.RefreshToken,
    redirectUri: "https://app.example.com");
```

### Работа со сделками

```csharp
// Создание сделки
var requests = new List<AddLeadRequest>
{
    new AddLeadRequest
    {
        Name = name,
        Price = price
    }
};
            
await _amoCrmClient.AddLeadsAsync(accessToken, subdomain, requests);

// Получение сделок с поиском по заполненным полям
var leads = await _amoCrmClient.GetLeadsAsync(accessToken, subdomain, query: "123");

// Обновление сделки

var requests = new List<UpdateLeadRequest>
{
    new UpdateLeadRequest
    {
        Id = 123456,
        Price = 1000,
        Status = 142
    }
};

await _amoCrmClient.UpdateLeadsAsync(accessToken, subdomain, requests);
```

### Работа с задачами

```csharp
// Создание задачи
var tasks = await _amoCrmClient.AddTasksAsync(accessToken, subdomain, new[] { new AddTaskRequest("Task 1", completeTill: 1234567890, EntityTypeEnum.Leads) });

// Получение задач
var tasks = await _amoCrmClient.GetTasksAsync(accessToken, subdomain,);

// Обновление задачи
await _amoCrmClient.UpdateTasksAsync(accessToken, subdomain, new[] { new UpdateTaskRequest(1, "Updated Task", completeTill: DateTimeOffset.Now.ToUnixTimeSeconds()) });
```

### Пакетные операции

Клиент поддерживает пакетную отправку данных для оптимизации количества запросов к API:

```csharp
// Пакетное создание сделок
var leadsToAdd = new List<AddLeadRequest> {
    new() { Name = "Сделка 1", Price = 100000 },
    new() { Name = "Сделка 2", Price = 200000 }
};
await _amoCrmClient.AddLeadsAsync(accessToken, subdomain, leadsToAdd);
```

## Расширенные примеры тестирования

### Unit тестирование с FluentAssertions

```csharp
[Fact]
public async Task AddTasksAsync_NullRequests_ThrowsArgumentNullException()
{
    IReadOnlyCollection<AddTaskRequest> requests = null!;

    await FluentActions
        .Invoking(async () => await Client.AddTasksAsync(TestAccessToken, TestSubdomain, requests).ConfigureAwait(false))
        .Should().ThrowAsync<ArgumentNullException>();
}
```

```csharp
[Fact]
    public void CreateForWidget_ReturnsCorrectUriBuilder()
    {
        // Arrange
        var widgetCode = "W001";

        // Act
        UriBuilder uriBuilder = _factory.CreateForWidget(TestSubdomain, widgetCode);

        // Assert
        uriBuilder.Scheme.Should().Be("https");
        uriBuilder.Host.Should().Be(TestSubdomain);
        uriBuilder.Path.Should().Be($"api/v4/widgets/{widgetCode}");
    }
```

### Моки для тестирования

```csharp
public class AmoCrmUriBuilderFactoryTests
{
    protected const string TestClientId = "test-client-id";
    protected const string TestClientSecret = "test-client-secret";
    protected const string TestAccessToken = "access-token";
    protected const string TestSubdomain = "example.amocrm.ru";
    protected const string TestRedirectUri = "https://example.com";

    private readonly AmoCrmUriBuilderFactory _factory;
    private readonly Mock<IOptions<AmoCrmClientOptions>> _optionsMock;

    public AmoCrmUriBuilderFactoryTests()
    {
        _optionsMock = new Mock<IOptions<AmoCrmClientOptions>>();
        var options = new AmoCrmClientOptions()
        {
            ClientId = TestClientId,
            ClientSecret = TestClientSecret,
            ServerIntegrationSubdomain = TestSubdomain,
            ServerIntegrationRedirectUri = TestRedirectUri
        };
        _optionsMock.Setup(o => o.Value).Returns(options);
        _factory = new AmoCrmUriBuilderFactory(_optionsMock.Object);
    }

    //...
}
```

## Конфигурация

### Базовая конфигурация

```json
{
  "AmoCrmClientOptions":
  {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "ServerIntegrationRedirectUri": "https://example.com/",
    "ServerIntegrationAuthCode": "YOUR_AUTH_CODE_",
    "ServerIntegrationSubdomain": "example.amocrm.ru"
  },
}
```

### Продвинутая конфигурация

```csharp
var options = new AmoCrmClientOptions
{
    ClientId = "YOUR_CLIENT_ID",
    ClientSecret = "YOUR_CLIENT_SECRET",
    ServerIntegrationRedirectUri = "YOUR_REDIRECT_URI",
    ServerIntegrationAuthCode = "YOUR_AUTH_CODE",
    ServerIntegrationSubdomain = "YOUR_SUBDOMAIN",
    TimeoutSeconds = 90,
    MaxCacheSize = 2000,
    Retry = new RetryOptions()
    {
        BaseDelay =  TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(10),
        JitterFactor = 0.2,
        MaxRetries = 5
    },
    CircuitBreaker = new CircuitBreakerOptions()
    {
        Enabled = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development",
        FailuresBeforeOpen = 3,
        OpenDuration = TimeSpan.FromSeconds(60)
    }
```

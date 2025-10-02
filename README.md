# AmoCrm.Client

[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0%20%7C%209.0-blue)](https://dotnet.microsoft.com/)

Идиоматичный и типобезопасный .NET клиент для работы с [amoCRM API v4](https://www.amocrm.ru/developers/content/crm_platform/api-reference) с поддержкой кэширования и OAuth 2.0 авторизации.

## Особенности

- 🛠️ **Типобезопасность** - строго типизированные модели для всех сущностей amoCRM
- 🔐 **OAuth 2.0** - поддержка клиентских и серверных интеграций
- 🛡️ **Надежность** - политики повтора, валидация данных, обработка ошибок
- 💾 **HTTP-кеширование** - прозрачное кеширование ответов API (10 минут TTL)
- 📝 **Структурное логирование** - Microsoft.Extensions.Logging
- ⚙️ **Конфигурируемость** - настройка через appsettings.json с поддержкой options pattern
- 🧩 **DI интеграция** - готовые расширения для ASP.NET Core
- 🧪 **Тестируемость** - проект включает набор unit-тестов, все зависимости легко мокаются

## Поддержка платформ

- .NET 6.0+
- .NET 8.0+
- .NET 9.0+

## Установка

```bash
dotnet add package Planfact.AmoCrm.Client
```

## Быстрый старт

### 1. Регистрация в DI контейнере

```csharp
// Базовый клиент без кеширования
services.AddAmoCrmClient(Configuration);
// Кешированный клиент (рекомендуется)
services.AddCachedAmoCrmClient(Configuration);
```

### 2. Конфигурация через appsettings.json

```json
{
  "AmoCrmClientOptions": {
    "ClientId": "<client_id_>",
    "ClientSecret": "<client_secret>",
    "ServerIntegrationRedirectUri": "https://example.ru/",
    "ServerIntegrationAuthCode": "<auth_code>",
    "ServerIntegrationSubdomain": "example.amocrm.ru",
    "TimeoutSeconds": 30
  }
}
```

### 3. Использование

```csharp
public class AccountService
{
    private readonly IAmoCrmClient _amoCrmClient;
    public AccountService(IAmoCrmClient amoCrmClient)
    {
        _amoCrmClient = amoCrmClient;
    }
    public async Task<AccountBusinessModel> GetAccountAsync(string accessToken, string subdomain)
    {    
        AccountResponse account = await _amoCrmClient.GetAccountAsync(accessToken, subdomain);

        return new AccountBusinessModel(account);
    }
}
```

## Структура библиотеки

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
├── AmoCrmClient.cs                 # Базовая реализация
└── CachedAmoCrmClient.cs           # Реализация с кешированием
```

## Основные операции

### Авторизация

```csharp
// Авторизация через OAuth 2.0
var tokens = await _amoCrmClient.Authorization.AuthorizeExternalAsync(
    subdomain: "example",
    authCode: "auth_code",
    redirectUri: "https://app.example.com"
);

// Обновление токенов доступа
var newTokens = await _amoCrmClient.RefreshTokenExternalAsync(
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

## Пакетные операции

Сервисы поддерживают пакетную отправку данных для оптимизации количества запросов к API:

```csharp
// Пакетное создание сделок
var leadsToAdd = new List<AddLeadRequest> {
    new() { Name = "Сделка 1", Price = 100000 },
    new() { Name = "Сделка 2", Price = 200000 }
};
await _amoCrmClient.AddLeadsAsync(accessToken, subdomain, leadsToAdd);
```

## Кеширование

`CachedAmoCrmClient` автоматически кеширует GET-запросы и инвалидирует кеш при мутациях:

```csharp
// Автоматически кешируется
var tasks = await _amoCrmClient.GetTasksInternalAsync(...);
// Кэш задач автоматически инвалидируется
await _amoCrmClient.AddTasksInternalAsync(...);
```

### Параметры кеша

- **TTL**: 10 минут (MediumTerm preset)
- **Размер кеша**: 1,000 записей
- **Автоматическое управление**: очистка старых записей
- **Прозрачность**: кеширование на уровне HTTP ответов

### Ограничения

В текущей реализации используется `MemoryCache`, который не поддерживает в полной мере инвалидацию кеша, в том числе по шаблону.
Таким образом, кешировать ответы методов API, связанных с "мутируемыми" сущностями (сделки, покупатели, компании, контакты, примечания) нецелесообразно
Поэтому, при настройке кешированного клиента применяется фильтр безопасных методов, которые работают с неизменяемыми (с точки зрения клиента) данными

```csharp
private static bool IsPathAllowedForCaching(Uri? requestUri, AmoCrmClientOptions options)
{
    if (requestUri is null)
    {
        return false;
    }

    // Кэшируем только безопасные эндпоинты (справочники, метаданные, пользователи)
    string[] pathSegmentsAllowedToCache =
    [
        options.OAuthTokenPath,
        options.AccountsApiPath,
        options.WidgetsApiPath,
        options.UsersApiPath,
        "/custom_fields",
        "/pipelines",
    ];

    var absolutePath = requestUri.AbsolutePath;
    return pathSegmentsAllowedToCache.Any(segment =>
        absolutePath.StartsWith(segment, StringComparison.OrdinalIgnoreCase) ||
        absolutePath.Contains($"/{segment.Trim('/')}/", StringComparison.OrdinalIgnoreCase));
}
```

## Обработка ошибок

Клиент использует типизированные исключения для различных типов ошибок:

```csharp
private static void HandleHttpError(HttpResponseMessage response, string responseContent)
{
    var statusCode = (int)response.StatusCode;
    var errorMessage = GetStatusCodeDescription(response.StatusCode);

    if (!string.IsNullOrWhiteSpace(responseContent))
    {
        errorMessage += $" Содержимое ответа: {responseContent}";
    }

    throw response.StatusCode switch
    {
        HttpStatusCode.BadRequest => new AmoCrmValidationException(errorMessage),
        HttpStatusCode.Unauthorized => new AmoCrmAuthenticationException(errorMessage),
        HttpStatusCode.PaymentRequired => new AmoCrmHttpException(errorMessage),
        HttpStatusCode.Forbidden => new AmoCrmAuthenticationException(errorMessage),
        _ => new AmoCrmHttpException(errorMessage, statusCode)
    };
}
```

## Тестирование

В текущей реализации проект содержит более 500 unit тестов, обеспечивающих покрытие основной функциональности.
В ходе развития проекта будут реализованы интеграционные тесты и тесты контракта API

### Запуск тестов

```shell
dotnet test Planfact.AmoCrm.Client.Tests
```

## Лицензия

MIT License. См. [LICENSE](LICENSE) для деталей.

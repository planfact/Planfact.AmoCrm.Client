# Конфигурирование

## Аутентификация и типы интеграции

Planfact.AmoCrm.Client поддерживает аутентификацию по протоколу OAuth 2.0 с использованием клиентских ключей.
Состав необходимых настроек для получения `access_token` и `refresh_token` зависит от типа интеграции.
Planfact.AmoCrm.Client поддерживает 2 вида интеграции - серверную и клиентскую.

> 📖 Полное описание процесса аутентификации и возможностей интеграций доступно в официальной документации:
- [amoCRM OAuth 2.0](https://www.amocrm.ru/developers/content/oauth/oauth);
- [Интеграции](https://www.amocrm.ru/developers/content/crm_platform/platform-abilities).

### 1. Серверная интеграция

Серверная интеграция предназначена для взаимодействия с конкретным аккаунтом в amoCRM по предварительно настроенным redirect URI, коду авторизации и поддомену.

```csharp
var options = new AmoCrmClientOptions
{
    ClientId = "YOUR_CLIENT_ID",
    ClientSecret = "YOUR_CLIENT_SECRET",
    ServerIntegrationRedirectUri = "YOUR_REDIRECT_URI",   // URI для перенаправления, указанный при создании интеграции ЛК amoCRM
    ServerIntegrationAuthCode = "YOUR_AUTH_CODE"   // код авторизации,
    ServerIntegrationSubdomain = "YOUR_SUBDOMAIN"  // поддомен акаунта, с которым связана интеграция
};
```

### 2. Клиентская интеграция

Клиентская интеграция работает через виджеты, опубликованные в маркетплейсе amoCRM
Для поддержки данного типа интеграции укажите код виджета и связанные параметры:

```csharp
var options = new AmoCrmClientOptions
{
    ClientId = "YOUR_CLIENT_ID",
    ClientSecret = "YOUR_CLIENT_SECRET",
    WidgetCode = "YOUR_WIDGET_CODE",  // Код виджета, опубликованного в маркетплейсе amoCRM
    WidgetApiKeyFieldName = "YOUR_WIDGET_API_KEY_FIELD",  // Название поля ключа интеграции в виджете
    WidgetCallbackPath = "/api/v1/amocrm/oauth/webhook"  // Путь для получения авторизационных данных после подключения виджета
};
```

## Параметры конфигурации

### AmoCrmClientOptions

Основной класс для настройки клиента API amoCRM. Наследует от `HttpClientOptions` и поддерживает следующие параметры:

Класс для настройки клиента API amoCRM, наследующий `HttpClientOptions`. Поддерживаемые параметры:

| Параметр                          | Описание                                                                 | По умолчанию                                 |
|-----------------------------------|--------------------------------------------------------------------------|-------------------------------------------   |
| **ClientId**                      | Идентификатор клиентского приложения для OAuth (Integration ID)          | -                                            |
| **ClientSecret**                  | Секретный ключ клиентского приложения для OAuth (Secret key)             | -                                            |
| **WidgetCode**                    | Код виджета для интеграции через виджеты amoCRM                          | -                                            |
| **WidgetApiKeyFieldName**         | Название поля виджета для хранения API-ключа                             | -                                            |
| **WidgetCallbackPath**            | Путь для OAuth callback от виджетов                                      | /api/v1/amocrmIntegrationWidget/OAuthWebhook |
| **ServerIntegrationRedirectUri**  | URI для OAuth redirect при серверной интеграции                          | -                                            |
| **ServerIntegrationAuthCode**     | Код авторизации для серверной интеграции                                 | -                                            |
| **ServerIntegrationSubdomain**    | Поддомен amoCRM для серверной интеграции                                 | -                                            |
| **OAuthTokenPath**                | Путь к OAuth token endpoint                                              | oauth2/access_token                          |
| **BaseApiPath**                   | Базовый путь для API v4 endpoints                                        | api/v4                                       |
| **LeadsApiPath**                  | Путь к API сделок                                                        | api/v4/leads                                 |
| **CustomersApiPath**              | Путь к API покупателей                                                   | api/v4/customers                             |
| **AccountsApiPath**               | Путь к API аккаунта                                                      | api/v4/account                               |
| **TasksApiPath**                  | Путь к API задач                                                         | api/v4/tasks                                 |
| **CompaniesApiPath**              | Путь к API компаний                                                      | api/v4/companies                             |
| **WidgetsApiPath**                | Путь к API виджетов                                                      | api/v4/widgets                               |
| **UsersApiPath**                  | Путь к API пользователей                                                 | api/v4/users                                 |
| **ContactsApiPath**               | Путь к API контактов                                                     | api/v4/contacts                              |
| **TransactionsApiPath**           | Путь к API транзакций                                                    | api/v4/customers/transactions                |
| **PipelinesApiPath**              | Путь к API воронок                                                       | api/v4/leads/pipelines                       |
| **CustomFieldsApiResourceName**   | Название ресурса дополнительных полей в API amoCRM                       | custom_fields                                |
| **TransactionsApiResourceName**   | Название ресурса транзакций в API amoCRM                                 | transactions                                 |
| **NotesApiResourceName**          | Название ресурса примечаний в API amoCRM                                 | notes                                        |
| **CacheExpiryMinutes**            | Время жизни кэша в минутах (для `CachedAmoCrmClient`)                    | 10                                           |
| **MaxCacheSize**                  | Максимальный размер кэша в записях (для `CachedAmoCrmClient`)            | 1000                                         |
| **UserAgent**                     | User-Agent для HTTP-запросов                                             | AmoCrm-DotNet-Client/1.0                     |
| **TimeoutSeconds**                | Таймаут HTTP-запроса в секундах                                          | 120                                          |

---

> 📖 Полное описание класса см. в исходном коде: `src/Planfact.AmoCrm.Client/AmoCrmClientOptions.cs`

## Интеграция с ASP.NET Core

### Базовая настройка

Регистрация клиента без кэширования:

```csharp
// Program.cs
builder.Services.AddAmoCrmClient(builder.Configuration);

// appsettings.json
{
  "AmoCrmClientOptions": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "ServerIntegrationSubdomain": "YOUR_SUBDOMAIN"
  }
}
```

### Настройка с кэшированием

Регистрация клиента с кэшированием HTTP-ответов (рекомендуется для операций чтения):

```csharp
// Program.cs
builder.Services.AddCachedAmoCrmClient(builder.Configuration);

// appsettings.json
{
  "AmoCrmClientOptions": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "ServerIntegrationSubdomain": "YOUR_SUBDOMAIN",
    "CacheExpiryMinutes": 10,
    "MaxCacheSize": 1000
  }
}
```

### Настройка с управляемым HttpClient

Библиотека автоматически настраивает `HttpClient` через `AddAmoCrmClient` или `AddCachedAmoCrmClient`:

```csharp
// HttpClient автоматически настраивается с:
// - Timeout: 120 секунд (или как указано в TimeoutSeconds)
// - User-Agent: AmoCrm-DotNet-Client/1.0 (или как указано в UserAgent)
// - Resilience: retry policies, circuit breaker, exponential backoff
```

## Конфигурация для разных сред

### Development

```json
{
  "AmoCrmClientOptions": {
    "ClientId": "dev-client-id",
    "ClientSecret": "dev-client-secret",
    "ServerIntegrationSubdomain": "dev-subdomain",
    "TimeoutSeconds": 30,
    "CacheExpiryMinutes": 5
  }
}
```

### Production

```json
{
  "AmoCrmClientOptions": {
    "ClientId": "prod-client-id",
    "ClientSecret": "prod-client-secret",
    "ServerIntegrationSubdomain": "prod-subdomain",
    "TimeoutSeconds": 120,
    "CacheExpiryMinutes": 10,
    "MaxCacheSize": 1000
  }
}
```

## Кэширование

`AddCachedAmoCrmClient` автоматически кэширует ответы безопасных GET и HEAD запросов к следующим эндпоинтам:
- Аутентификация (`oauth2/access_token`)
- Аккаунт (`api/v4/account`)
- Виджеты (`api/v4/widgets`)
- Пользователи (`api/v4/users`)
- Пользовательские поля (`/custom_fields`)
- Воронки (`/pipelines`)

### Параметры кэша

- **TTL**: Настраивается через `CacheExpiryMinutes` (по умолчанию: 10 минут)
- **Размер кэша**: Настраивается через `MaxCacheSize` (по умолчанию: 1000 записей)
- **Автоматическое управление**: Очистка старых записей через `MemoryCache`
- **Фильтрация**: Кэшируются только успешные ответы (2xx) для безопасных эндпоинтов

```csharp
// Пример настройки кэшированного клиента
builder.Services.AddCachedAmoCrmClient(builder.Configuration);
```

### Ограничения кэширования

- Используется `MemoryCache`, который не поддерживает сложную инвалидацию кэша по шаблонам.
- Кэшируются только безопасные эндпоинты (справочники, метаданные, пользователи).
- Мутационные операции (создание/обновление сделок, контактов и т.д.) не кэшируются.

## Лучшие практики

1. **Безопасность**: Храните `ClientId`, `ClientSecret`, `ServerIntegrationAuthCode` в секретах (например, Azure Key Vault, AWS Secrets Manager) или переменных окружения. Обновляйте `refresh_token` своевременно для предотвращения реавторизации.
2. **Надежность**: Используйте стандартную регистрацию `AddAmoCrmClient()` для автоматических resilience patterns. Для кэширования применяйте CachedAmoCrmClient в высоконагруженных сценариях.
3. **Мониторинг**: Настройте логирование через `Microsoft.Extensions.Logging` для отслеживания ошибок и результатов выполнения запросов.
4. **Тестирование**: Установите меньшие значения `TimeoutSeconds` и `CacheExpiryMinutes` в тестовой среде. Используйте моки для `IAmoCrmClient` в юнит-тестах.
5. **Развертывание**: Настройте `ServerIntegrationSubdomain` и `ServerIntegrationRedirectUri` для аккаунта.
6. **Production**: Доверьтесь встроенным resilience patterns для стабильной работы с API.

### Отказоустойчивость

Библиотека автоматически настраивает resilience patterns через `Reliable.HttpClient`:

```csharp
// Автоматическая настройка (через AddResilience())
builder.Services.AddAmoCrmClient(builder.Configuration);
// или
builder.Services.AddCachedAmoCrmClient(builder.Configuration);
```

Встроенные resilience patterns обеспечивают:

- **Автоматические повторы** при временных сбоях (например, сетевые ошибки)
- **Circuit breaker** для защиты от каскадных отказов
- **Exponential backoff** с jitter для оптимального retry

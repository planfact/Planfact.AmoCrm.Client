# AmoCrm.Client

[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%209.0-blue)](https://dotnet.microsoft.com/)

Типобезопасный .NET клиент для работы с [amoCRM API v4](https://www.amocrm.ru/developers/content/crm_platform/api-reference) с поддержкой кэширования и OAuth 2.0 авторизации.

## Особенности

- **Типобезопасность** - строго типизированные модели для всех сущностей amoCRM
- **OAuth 2.0** - поддержка клиентских и серверных интеграций
- **Надежность** - политики повтора, валидация данных, обработка ошибок
- **HTTP-кэширование** - прозрачное кеширование GET-запросов с автоматической инвалидацией
- **Мульти-таргетинг** - поддержка .NET 6.0 и 9.0
- **Логирование** - структурированное логирование всех операций
- **Конфигурируемость** - гибкая настройка через appsettings.json или код
- **DI интеграция** - готовые расширения для ASP.NET Core

## Установка

dotnet add package Planfact.AmoCrm.Client

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
    "BaseUrl": "https://yourdomain.amocrm.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RedirectUri": "https://your-redirect-uri",
    "AccessToken": "your-access-token",
    "RefreshToken": "your-refresh-token",
    "TimeoutSeconds": 30
  }
}
```

### 3. Использование

```csharp
public class LeadService
{
    private readonly IAmoCrmClient _amoCrmClient;
    public LeadService(IAmoCrmClient amoCrmClient)
    {
        _amoCrmClient = amoCrmClient;
    }
    public async Task<Lead> CreateLeadAsync(string name, decimal price)
    {
        var lead = new Lead
        {
            Name = name,
            Price = price
        };
        var response = await _amoCrmClient.AddLeadAsync(lead);
        return response;
    }
}
```

## Структура библиотеки

```shell
├── IAmoCrmClient.cs            # Основной интерфейс клиента
├── AmoCrmClient.cs             # Базовая реализация клиента
├── CachedAmoCrmClient.cs       # Реализация с кешированием
├── Configuration/              # Настройки и конфигурация
│   ├── AmoCrmClientOptions.cs  # Опции клиента
│   └── ServiceCollectionExtensions.cs # DI расширения
├── Models/                     # Модели данных
│   ├── Lead.cs                # Сделки
│   ├── Task.cs                # Задачи
│   ├── Contact.cs             # Контакты
│   └── Company.cs             # Компании
├── Enums/                     # Перечисления
│   ├── EntityTypeEnum.cs # Типы сущностей
│   └── AmoCrmNoteTypeEnum.cs   # Типы примечаний
├── Requests/                  # Запросы к API
│   ├── AddLeadRequest.cs      # Создание сделки
│   └── GetTasksRequest.cs     # Получение задач
└── Utils/                    # Вспомогательные классы
└── AmoCrmUriBuilderFactory.cs # Построитель URL
```

## Основные операции

```csharp
// Авторизация через виджет (клиентская интеграция)
var tokens = await _amoCrmClient.AuthorizeExternalAsync( subdomain: "example", authorizationCode: "auth_code", redirectDomain: "https://app.example.com");

// Авторизация через внутреннюю интеграцию
var tokens = await _amoCrmClient.AuthorizeInternalAsync();

// Обновление токенов доступа
var newTokens = await _amoCrmClient.RefreshTokenExternalAsync( subdomain: "example", refreshToken: tokens.RefreshToken, redirectDomain: "https://app.example.com");
```

### Работа со сделками

```csharp
// Создание сделки
var leads = await _amoCrmClient.AddLeadsAsync( accessToken, subdomain, new[] { new AddLeadRequest { Name = "Новая сделка", Price = 100000, ResponsibleUserId = 123, StatusId = 456 } });

// Получение сделок
var leads = await _amoCrmClient.GetLeadsAsync( accessToken, subdomain, new GetLeadsRequest { Page = 1, PageSize = 250, OrderBy = "created_at", OrderDirection = "desc" });

// Обновление сделки
await _amoCrmClient.UpdateLeadsAsync( accessToken, subdomain, new[] { new UpdateLeadRequest { Id = 789, StatusId = 321, Price = 150000 } });
```

### Работа с задачами

```csharp
// Создание задачи
var tasks = await _amoCrmClient.AddTasksAsync( accessToken, subdomain, new[] { new AddTaskRequest { Text = "Позвонить клиенту", CompleteTill = DateTime.UtcNow.AddDays(1), EntityId = 123, EntityType = EntityTypeEnum.Leads, ResponsibleUserId = 456 } });

// Получение задач
var tasks = await _amoCrmClient.GetTasksAsync( accessToken, subdomain, new GetTasksRequest { Page = 1, PageSize = 250, EntityType = EntityTypeEnum.Leads });

// Обновление задачи
await _amoCrmClient.UpdateTasksAsync( accessToken, subdomain, new[] { new UpdateTaskRequest { Id = 789, IsCompleted = true, Result = new TaskResult { Text = "Клиент согласен" } } });
```

## Пакетные операции

Клиент поддерживает пакетную отправку данных для оптимизации запросов:

```csharp
// Пакетное создание сделок
var leads = new List<AddLeadRequest> { new() { Name = "Сделка 1", Price = 100000 }, new() { Name = "Сделка 2", Price = 200000 } };
await _amoCrmClient.AddLeadsAsync(accessToken, subdomain, leads)
```

## Кеширование

`CachedAmoCrmClient` автоматически кеширует GET-запросы и инвалидирует кэш при мутациях:

```csharp
// Автоматически кешируется
var tasks = await _amoCrmClient.GetTasksAsync(...);
// Кэш задач автоматически инвалидируется
await _amoCrmClient.AddTasksAsync(...);
```csharp

## Тестирование

### Запуск тестов

```shell
dotnet test Planfact.AmoCrm.Client
```

## Логирование

Клиент поддерживает структурное логирование через `Microsoft.Extensions.Logging` для  всех операций.

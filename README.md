# AmoCrm.Client

[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%208.0%20%7C%209.0-blue)](https://dotnet.microsoft.com/)

Идиоматичный и типобезопасный .NET клиент для работы с [amoCRM API v4](https://www.amocrm.ru/developers/content/crm_platform/api-reference) с поддержкой кэширования и OAuth 2.0 авторизации.

## Особенности

- 🛠️ **Типобезопасность** - строго типизированные модели для всех сущностей amoCRM
- 🔐 **OAuth 2.0** - поддержка клиентских и серверных интеграций
- 🛡️ **Надежность** - политики повтора, валидация данных, обработка ошибок
- 💾 **HTTP-кэширование** - прозрачное кэширование ответов API (10 минут TTL)
- 📝 **Структурное логирование** - Microsoft.Extensions.Logging
- ⚙️ **Гибкая конфигурация** - настройка как через код, так и через appsettings.json с поддержкой options pattern
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

```csharp
// Program.cs
builder.Services.AddAmoCrmClient(builder.Configuration);

// Внедрение зависимости
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

## Тестирование

В текущей реализации проект содержит более 500 unit тестов, обеспечивающих покрытие основной функциональности.
В ходе развития проекта будут реализованы интеграционные тесты и тесты контракта API

### Запуск тестов

```shell
dotnet test Planfact.AmoCrm.Client.Tests
```

## Документация

- [Конфигурирование](docs/Configuration.md)
- [Архитектура](docs/Architecture.md)

## Лицензия

MIT License. См. [LICENSE](LICENSE) для деталей.

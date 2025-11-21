# AmoCrm.Client

[![NuGet Version](https://img.shields.io/nuget/v/Planfact.AmoCrm.Client?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Planfact.AmoCrm.Client/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Planfact.AmoCrm.Client?style=flat-square&logo=nuget)](https://www.nuget.org/packages/Planfact.AmoCrm.Client/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/planfact/Planfact.AmoCrm.Client/ci.yml?branch=main&style=flat-square&logo=github)](https://github.com/planfact/Planfact.AmoCrm.Client/actions)
[![codecov](https://img.shields.io/codecov/c/github/planfact/Planfact.AmoCrm.Client?style=flat-square&logo=codecov)](https://codecov.io/gh/planfact/Planfact.AmoCrm.Client)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%7C8.0%7C9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![GitHub release](https://img.shields.io/github/v/release/planfact/Planfact.AmoCrm.Client?style=flat-square&logo=github)](https://github.com/planfact/Planfact.AmoCrm.Client/releases)

Идиоматичный и типобезопасный .NET клиент для работы с [amoCRM API v4](https://www.amocrm.ru/developers/content/crm_platform/api-reference) с поддержкой кэширования и OAuth 2.0.

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

В текущей реализации проект содержит более 800 unit и snapshot тестов, обеспечивающих покрытие основной функциональности.
В ходе развития проекта будут реализованы интеграционные тесты.

### Запуск тестов

```shell
dotnet test Planfact.AmoCrm.Client.Tests
```

## Документация

- [Конфигурирование](docs/Configuration.md)
- [Архитектура](docs/Architecture.md)

## Лицензия

MIT License. См. [LICENSE](LICENSE) для деталей.

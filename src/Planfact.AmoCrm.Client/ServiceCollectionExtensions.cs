using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Reliable.HttpClient;
using Reliable.HttpClient.Caching;
using Reliable.HttpClient.Caching.Extensions;

using Planfact.AmoCrm.Client.Account;
using Planfact.AmoCrm.Client.Authorization;
using Planfact.AmoCrm.Client.Companies;
using Planfact.AmoCrm.Client.Leads;
using Planfact.AmoCrm.Client.Tasks;
using Planfact.AmoCrm.Client.Customers;
using Planfact.AmoCrm.Client.Users;
using Planfact.AmoCrm.Client.Contacts;
using Planfact.AmoCrm.Client.Transactions;
using Planfact.AmoCrm.Client.CustomFields;
using Planfact.AmoCrm.Client.Pipelines;
using Planfact.AmoCrm.Client.Notes;

namespace Planfact.AmoCrm.Client.Configuration;

/// <summary>
/// Расширения для регистрации клиентов amoCRM в контейнере зависимостей
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует базовый клиент amoCRM без кэширования
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="config">Конфигурация с секцией <see cref="AmoCrmClientOptions"/></param>
    /// <returns>Коллекция сервисов для дальнейшей настройки</returns>
    /// <exception cref="InvalidOperationException">Секция конфигурации не найдена</exception>
    public static IServiceCollection AddAmoCrmClient(
        this IServiceCollection services,
        IConfiguration config)
    {
        IConfigurationSection section = config.GetRequiredSection(nameof(AmoCrmClientOptions));
        services.Configure<AmoCrmClientOptions>(section);

        AmoCrmClientOptions amoCrmOptions = section.Get<AmoCrmClientOptions>()
            ?? throw new InvalidOperationException($"Конфигурация {nameof(AmoCrmClientOptions)} не найдена.");

        services.AddSingleton<IHttpResponseHandler, AmoCrmHttpResponseHandler>();
        services.AddSingleton<AmoCrmUriBuilderFactory>();

        services.AddHttpClient<IHttpClientAdapter, HttpClientAdapter>()
            .ConfigureHttpClient(ConfigureAmoCrmHttpClient)
            .AddResilience(HttpClientPresets.SlowExternalApi());

        services.AddHttpClientAdapter();
        
        services.AddScoped<IAmoCrmAccountService, AmoCrmAccountService>();
        services.AddScoped<IAmoCrmAuthorizationService, AmoCrmAuthorizationService>();
        services.AddScoped<IAmoCrmLeadService, AmoCrmLeadService>();
        services.AddScoped<IAmoCrmCompanyService, AmoCrmCompanyService>();
        services.AddScoped<IAmoCrmTaskService, AmoCrmTaskService>();
        services.AddScoped<IAmoCrmCustomerService, AmoCrmCustomerService>();
        services.AddScoped<IAmoCrmUserService, AmoCrmUserService>();
        services.AddScoped<IAmoCrmContactService, AmoCrmContactService>();
        services.AddScoped<IAmoCrmTransactionService, AmoCrmTransactionService>();
        services.AddScoped<IAmoCrmCustomFieldService, AmoCrmCustomFieldService>();
        services.AddScoped<IAmoCrmPipelineService, AmoCrmPipelineService>();
        services.AddScoped<IAmoCrmNoteService, AmoCrmNoteService>();
        services.AddScoped<IAmoCrmClient, AmoCrmClient>();

        return services;
    }

    /// <summary>
    /// Регистрирует клиент amoCRM с кэшированием HTTP-ответов
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="config">Конфигурация с секцией <see cref="AmoCrmClientOptions"/></param>
    /// <returns>Коллекция сервисов для дальнейшей настройки</returns>
    /// <exception cref="InvalidOperationException">Секция конфигурации не найдена</exception>
    /// <remarks>
    /// Кэширует только операции чтения для безопасных эндпоинтов (авторизация, аккаунты, справочники).
    /// TTL и размер кэша настраиваются через параметры
    /// <see cref="AmoCrmClientOptions.CacheExpiryMinutes"/> и <see cref="AmoCrmClientOptions.MaxCacheSize"/>.
    /// </remarks>
    public static IServiceCollection AddCachedAmoCrmClient(
        this IServiceCollection services,
        IConfiguration config)
    {
        IConfigurationSection section = config.GetRequiredSection(nameof(AmoCrmClientOptions));
        services.Configure<AmoCrmClientOptions>(section);

        AmoCrmClientOptions options = section.Get<AmoCrmClientOptions>()
            ?? throw new InvalidOperationException($"Конфигурация {nameof(AmoCrmClientOptions)} не найдена.");

        services.AddResilientHttpClientWithCache(
            "AmoCrmCached",
            HttpClientPresets.SlowExternalApi(),
            cacheOptions =>
            {
                cacheOptions.DefaultExpiry = TimeSpan.FromMinutes(options.CacheExpiryMinutes);
                cacheOptions.MaxCacheSize = options.MaxCacheSize;
                cacheOptions.DefaultHeaders = new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase) { { "User-Agent", options.UserAgent } };

                // Reliable.HttpClient.Caching автоматически:
                // - Кэширует только успешные ответы (2xx)
                // - Учитывает Cache-Control и Expires заголовки
                // - Генерирует безопасные ключи кэша (SHA256)
                // Мы добавляем только фильтрацию безопасных эндпоинтов amoCRM
                cacheOptions.ShouldCache = CreateCachingPredicate(options);
            }
        );

        services.AddSingleton<IHttpResponseHandler, AmoCrmHttpResponseHandler>();
        services.AddSingleton<AmoCrmUriBuilderFactory>();
        services.AddCachedAccountServiceFactory();
        services.AddCachedAuthorizationServiceFactory();
        services.AddCachedLeadServiceFactory();
        services.AddCachedCompanyServiceFactory();
        services.AddCachedTaskServiceFactory();
        services.AddCachedCustomerServiceFactory();
        services.AddCachedUserServiceFactory();
        services.AddCachedContactServiceFactory();
        services.AddCachedTransactionServiceFactory();
        services.AddCachedCustomFieldServiceFactory();
        services.AddCachedPipelineServiceFactory();
        services.AddCachedNoteServiceFactory();
        services.AddScoped<IAmoCrmClient, CachedAmoCrmClient>();

        return services;
    }

    private static IServiceCollection AddCachedAccountServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmAccountService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmAccountService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmAccountService>>();
            return httpClient => new AmoCrmAccountService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedAuthorizationServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmAuthorizationService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            IOptions<AmoCrmClientOptions> options = serviceProvider.GetRequiredService<IOptions<AmoCrmClientOptions>>();
            ILogger<AmoCrmAuthorizationService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmAuthorizationService>>();
            return httpClient => new AmoCrmAuthorizationService(httpClient, uriBuilderFactory, options, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedLeadServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmLeadService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmLeadService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmLeadService>>();
            return httpClient => new AmoCrmLeadService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedCompanyServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmCompanyService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmCompanyService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmCompanyService>>();
            return httpClient => new AmoCrmCompanyService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedTaskServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmTaskService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmTaskService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmTaskService>>();
            return httpClient => new AmoCrmTaskService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedCustomerServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmCustomerService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmCustomerService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmCustomerService>>();
            return httpClient => new AmoCrmCustomerService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedUserServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmUserService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmUserService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmUserService>>();
            return httpClient => new AmoCrmUserService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedContactServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmContactService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmContactService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmContactService>>();
            return httpClient => new AmoCrmContactService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedTransactionServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmTransactionService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmTransactionService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmTransactionService>>();
            return httpClient => new AmoCrmTransactionService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedCustomFieldServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmCustomFieldService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmCustomFieldService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmCustomFieldService>>();
            return httpClient => new AmoCrmCustomFieldService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedPipelineServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmPipelineService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmPipelineService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmPipelineService>>();
            return httpClient => new AmoCrmPipelineService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    private static IServiceCollection AddCachedNoteServiceFactory(this IServiceCollection services)
    {
        services.AddScoped<Func<HttpClientWithCache, IAmoCrmNoteService>>(serviceProvider =>
        {
            AmoCrmUriBuilderFactory uriBuilderFactory = serviceProvider.GetRequiredService<AmoCrmUriBuilderFactory>();
            ILogger<AmoCrmNoteService> logger = serviceProvider.GetRequiredService<ILogger<AmoCrmNoteService>>();
            return httpClient => new AmoCrmNoteService(httpClient, uriBuilderFactory, logger);
        });

        return services;
    }

    /// <summary>
    /// Настраивает HttpClient для работы с API amoCRM
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов</param>
    /// <param name="httpClient">HTTP-клиент для настройки</param>
    private static void ConfigureAmoCrmHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
    {
        // Получаем конфигурацию из DI
        AmoCrmClientOptions options = serviceProvider.GetRequiredService<IOptions<AmoCrmClientOptions>>().Value;
        httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);

        if (!string.IsNullOrWhiteSpace(options.UserAgent))
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(options.UserAgent);
        }
    }

    /// <summary>
    /// Создает предикат для определения возможности кэширования HTTP-запросов к amoCRM API
    /// </summary>
    /// <param name="options">Настройки клиента amoCRM</param>
    /// <returns>Функция для определения возможности кэширования запроса и ответа</returns>
    /// <remarks>
    /// Reliable.HttpClient.Caching автоматически обрабатывает:
    /// - Кэширование только успешных ответов (CacheOnlySuccessfulResponses = true)
    /// - Учет Cache-Control заголовков (RespectCacheControlHeaders = true)
    /// - Поддержку Expires заголовков
    /// Здесь мы добавляем только логику для фильтрации безопасных эндпоинтов amoCRM
    /// </remarks>
    private static Func<HttpRequestMessage, HttpResponseMessage, bool> CreateCachingPredicate(AmoCrmClientOptions options)
    {
        return (request, response) =>
        {
            // Кэшируем только GET и HEAD запросы (безопасные операции чтения)
            if (request.Method != HttpMethod.Get && request.Method != HttpMethod.Head)
            {
                return false;
            }

            // Проверяем, что URL запроса относится к разрешенным для кэширования эндпоинтам
            return IsPathAllowedForCaching(request.RequestUri, options);
        };
    }

    /// <summary>
    /// Проверяет, разрешен ли путь запроса для кэширования
    /// </summary>
    /// <param name="requestUri">URI запроса</param>
    /// <param name="options">Настройки клиента amoCRM</param>
    /// <returns>true, если путь разрешен для кэширования</returns>
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
}

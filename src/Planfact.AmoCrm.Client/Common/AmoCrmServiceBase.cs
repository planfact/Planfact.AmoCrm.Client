using System.Runtime.CompilerServices;

using Reliable.HttpClient;

namespace Planfact.AmoCrm.Client.Common;

/// <summary>
/// Абстрактный базовый класс для сервисов бизнес-сущностей AmoCRM, предоставляющий общие методы и логику
/// </summary>
/// <remarks>
/// Конструктор базового класса сервиса
/// </remarks>
/// <param name="httpClient">HTTP клиент</param>
/// <param name="logger">Логгер</param>
public abstract class AmoCrmServiceBase(IHttpClientAdapter httpClient, ILogger logger)
{
    /// <summary>
    /// Предельный размер пакета при пакетной отправке данных в amoCRM.
    /// Определяет максимальный размер массива сущностей (сделки, покупатели и пр.),
    /// который можно отправить за 1 запрос синхронизации (Add, Update, Delete) без риска блокировки интеграции
    /// <see href="https://www.amocrm.ru/developers/content/api/recommendations"> Правила использования API amoCRM</see>
    /// </summary>
    protected const int MaxEntitiesPerBatch = 50;

    /// <summary>
    /// Значение по умолчанию для query-параметра page
    /// </summary>
    protected const int PaginationStartPage = 1;

    /// <summary>
    /// Предельный размер страницы при загрузке данных из amoCRM с пагинацией
    /// <see href="https://www.amocrm.ru/developers/content/api/recommendations"> Правила использования API amoCRM</see>
    /// </summary>
    protected const int PaginationPerPageLimit = 250;

    /// <summary>
    /// HTTP клиент для отправки запросов в AmoCRM
    /// </summary>
    protected readonly IHttpClientAdapter HttpClient = httpClient;

    /// <summary>
    /// Логгер для внутреннего логирования событий и ошибок.
    /// </summary>
    protected readonly ILogger Logger = logger;

    /// <summary>
    /// Добавляет стандартные параметры пагинации amoCRM к URI
    /// </summary>
    /// <param name="uri">URI</param>
    /// <returns>Новый объект URI, созданный с учетом query-параметров пагинации</returns>
    private static Uri AddPaginationParameters(Uri uri)
    {
        var uriBuilder = new UriBuilder(uri)
        {
            Query = string.IsNullOrEmpty(uri.Query)
                ? $"page={PaginationStartPage}&limit={PaginationPerPageLimit}"
                : $"{uri.Query}&page={PaginationStartPage}&limit={PaginationPerPageLimit}"
        };

        return uriBuilder.Uri;
    }

    /// <summary>
    /// Добавляет параметр поисковой строки к URI
    /// </summary>
    /// <param name="uri">URI</param>
    /// <param name="query">Поисковый запрос</param>
    /// <returns>Новый объект URI, созданный с учетом query-параметра поискового запроса</returns>
    private protected static Uri AddSearchQueryParameter(Uri uri, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return uri;
        }

        var uriBuilder = new UriBuilder(uri)
        {
            Query = string.IsNullOrEmpty(uri.Query)
                ? $"query={Uri.EscapeDataString(query.Trim())}"
                : $"{uri.Query}&query={Uri.EscapeDataString(query.Trim())}"
        };

        return uriBuilder.Uri;
    }

    /// <summary>
    /// Получает стандартные заголовки для запросов к API amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа</param>
    /// <returns>Словарь заголовков</returns>
    private protected static IDictionary<string, string> GetDefaultHeaders(string accessToken)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "Authorization", $"Bearer {accessToken}" },
        };
    }

    /// <summary>
    /// Валидирует параметры, необходимые для доступа к amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа</param>
    /// <param name="subdomain">Поддомен amoCRM</param>
    private protected static void ValidateCredentials(string accessToken, string subdomain)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new ArgumentException("Не задан Access Token", nameof(accessToken));
        }

        if (string.IsNullOrWhiteSpace(subdomain))
        {
            throw new ArgumentException("Не задан поддомен amoCRM", nameof(subdomain));
        }
    }

    /// <summary>
    /// Обобщенная логика сбора сущностей из пагинированных ответов
    /// </summary>
    /// <typeparam name="T">Тип сущности (Lead, Company, AmoCrmTask и т.д.)</typeparam>
    /// <param name="paginationTask">Пагинированный поток ответов</param>
    /// <param name="extractor">Функция для извлечения коллекции из EntitiesResponse</param>
    /// <param name="subdomain">Поддомен amoCRM для логирования операции</param>
    /// <param name="operationDescription">Описание операции для логирования</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Собранная коллекция сущностей</returns>
    private protected async Task<IReadOnlyCollection<T>> CollectPaginatedEntitiesAsync<T>(
        IAsyncEnumerable<EntitiesResponse> paginationTask,
        Func<EntitiesResponse, IEnumerable<T>> extractor,
        string subdomain,
        string operationDescription,
        CancellationToken cancellationToken = default)
    {
        var entities = new List<T>();

        try
        {
            await foreach (EntitiesResponse response in paginationTask.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                IEnumerable<T> items = extractor(response);
                entities.AddRange(items);
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogDebug("{OperationDescription} в amoCRM отменено. Интеграция: {Subdomain}", operationDescription, subdomain);
            throw;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{OperationDescription} в amoCRM завершилось с ошибкой. Интеграция: {Subdomain}", operationDescription, subdomain);
            throw;
        }

        return entities.AsReadOnly();
    }

    /// <summary>
    /// Отправляет GET-запросы в API amoCRM до тех пор, пока доступна дальнейшая пагинация.
    /// Ответы API не буферизуются и доступны потребителю сразу после десериализации
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <param name="uri">URI запроса</param>
    /// <param name="accessToken">Токен доступа к API</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Коллекция десериализованных объектов ответа</returns>
    private protected async IAsyncEnumerable<TResponse> GetPaginatedAsync<TResponse>(
        Uri uri,
        string accessToken,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResponse : EntitiesResponse
    {
        Uri nextPageUri = AddPaginationParameters(uri);

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            IDictionary<string, string> headers = GetDefaultHeaders(accessToken);
            TResponse currentPage = await HttpClient.GetAsync<TResponse>(nextPageUri, headers, cancellationToken).ConfigureAwait(false);

            if (currentPage == default(TResponse))
            {
                yield break;
            }

            yield return currentPage;

            if (string.IsNullOrEmpty(currentPage.PaginationLinks?.Next?.Uri))
            {
                yield break;
            }

            nextPageUri = new Uri(currentPage.PaginationLinks.Next.Uri);
        }
    }

    /// <summary>
    /// Обновляет данные в API amoCRM через PATCH-запросы в пакетном режиме
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <param name="uri">URI запроса</param>
    /// <param name="accessToken">Токен доступа к API</param>
    /// <param name="payload">Данные, которые необходимо отправить в запросе</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Асинхронный поток десериализованных ответов API</returns>
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

    /// <summary>
    /// Отправляет данные в API amoCRM через POST-запросы в пакетном режиме
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <param name="uri">URI запроса</param>
    /// <param name="accessToken">Токен доступа к API</param>
    /// <param name="payload">Данные, которые необходимо отправить, разделив на пакеты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Асинхронный поток десериализованных ответов API</returns>
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

    /// <summary>
    /// Отправляет запросы в API amoCRM, разделяя данные на пакеты фиксированного размера
    /// </summary>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <param name="uri">URI запроса</param>
    /// <param name="accessToken">Токен доступа к API</param>
    /// <param name="payload">Данные, которые необходимо отправить, разделив на пакеты</param>
    /// <param name="sendMethod">Делегат для отправки пакета, определяет HTTP-метод, которым будут отправляться запросы,
    /// например, _httpClient.PostAsync или _httpClient.PatchAsync.
    /// Принимает следующие параметры:
    /// <list type="bullet">
    /// <item><description><c>uri</c> — строка URI запроса.</description></item>
    /// <item><description><c>batch</c> — массив данных запроса.</description></item>
    /// <item><description><c>headers</c> — словарь заголовков.</description></item>
    /// <item><description><c>cancellationToken</c> — токен отмены.</description></item>
    /// </list>
    /// </param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Асинхронный поток десериализованных ответов API</returns>
    private static async IAsyncEnumerable<TResponse> SendInBatchesAsync<TRequest, TResponse>(
        Uri uri,
        string accessToken,
        IReadOnlyCollection<TRequest> payload,
        Func<string, TRequest[], IDictionary<string, string>, CancellationToken, Task<TResponse>> sendMethod,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) where TResponse : class
    {
        IEnumerable<TRequest[]> batches = payload.Chunk(MaxEntitiesPerBatch);

        foreach (TRequest[] batch in batches)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            IDictionary<string, string> headers = GetDefaultHeaders(accessToken);
            TResponse response = await sendMethod(
                uri.ToString(),
                batch,
                headers,
                cancellationToken
            ).ConfigureAwait(false);

            yield return response;
        }
    }
}

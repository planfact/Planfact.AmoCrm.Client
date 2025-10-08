using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Customers;

/// <summary>
/// Сервис покупателей в amoCRM
/// </summary>
public sealed class AmoCrmCustomerService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmCustomerService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmCustomerService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmCustomerService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await GetCustomersAsync(accessToken, subdomain, [], query, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Customer>> GetCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка покупателей из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCustomers(subdomain);
        Uri requestUri = AddSearchQueryParameter(uriBuilder.Uri, query);
        requestUri = AddLinkedEntitiesParameters(requestUri, linkedEntityTypes);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            requestUri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<Customer> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Customers ?? [],
            subdomain,
            OperationDescriptions.GetCustomers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Покупатели из аккаунта {Subdomain} успешно загружены. Получено {CustomersCount} покупателей", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Customer>> AddCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Добавление покупателей в аккаунт {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCustomers(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddCustomerRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Customer> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Customers ?? [],
            subdomain,
            OperationDescriptions.AddCustomers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление покупателей в аккаунт {Subdomain} успешно завершено. Добавлено {AddedCustomersCount} покупателей из {RequestedCustomersCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Customer>> UpdateCustomersAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCustomerRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Редактирование покупателей в аккаунте {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCustomers(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateCustomerRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Customer> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Customers ?? [],
            subdomain,
            OperationDescriptions.UpdateCustomers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Редактирование покупателей в аккаунте {Subdomain} успешно завершено. Обновлено {UpdatedCustomersCount} покупателей из {RequestedCustomersCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }
}

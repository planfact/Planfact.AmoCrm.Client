using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Leads;

/// <summary>
/// Сервис сделок в amoCRM
/// </summary>
public sealed class AmoCrmLeadService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmLeadService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmLeadService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmLeadService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await GetLeadsAsync(accessToken, subdomain, [], query, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка сделок из аккаунта {Subdomain}. Поиск по вхождению {Query}", subdomain, query);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForLeads(subdomain);
        Uri requestUri = AddSearchQueryParameter(uriBuilder.Uri, query);
        requestUri = AddLinkedEntitiesParameters(requestUri, linkedEntityTypes);

        IReadOnlyCollection<Lead> response = await GetLeadsCoreAsync(
            requestUri,
            accessToken,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Сделки из аккаунта {Subdomain} загружены успешно. Получено {LeadsCount} сделок", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        return await GetLeadsAsync(accessToken, subdomain, [], ids, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> GetLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Загрузка сделок из аккаунта {Subdomain}. Поиск по идентификаторам {Ids}",
            subdomain,
            string.Join(", ", ids)
        );

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForLeads(subdomain);

        IEnumerable<string> filterQueryParameters = ids.Select((id, index) => $"filter[id][{index}]={id}");
        var filterQuery = string.Join('&', filterQueryParameters);
        uriBuilder.Query = string.Concat(uriBuilder.Query, filterQuery);

        Uri requestUri = AddLinkedEntitiesParameters(uriBuilder.Uri, linkedEntityTypes);

        IReadOnlyCollection<Lead> response = await GetLeadsCoreAsync(
            requestUri,
            accessToken,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Сделки из аккаунта {Subdomain} загружены успешно. Получено {LeadsCount} сделок",
            subdomain,
            response.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> AddLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Добавление сделок в аккаунт {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForLeads(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddLeadRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Lead> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Leads ?? [],
            subdomain,
            OperationDescriptions.AddLeads,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление сделок в аккаунт {Subdomain} успешно завершено. Добавлено {AddedLeadsCount} сделок из {RequestedLeadsCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Lead>> UpdateLeadsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateLeadRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Редактирование сделок в аккаунте {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForLeads(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateLeadRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Lead> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Leads ?? [],
            subdomain,
            OperationDescriptions.UpdateLeads,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Редактирование сделок в аккаунте {Subdomain} успешно завершено. Добавлено {UpdatedLeadsCount} сделок из {RequestedLeadsCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    private async Task<IReadOnlyCollection<Lead>> GetLeadsCoreAsync(
        Uri uri,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uri,
            accessToken,
            cancellationToken
        );

        var subdomain = uri.DnsSafeHost;

        return await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Leads ?? [],
            subdomain,
            OperationDescriptions.GetLeads,
            cancellationToken
        ).ConfigureAwait(false);
    }
}

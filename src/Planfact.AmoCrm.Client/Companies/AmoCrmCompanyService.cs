using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Companies;

/// <summary>
/// Сервис компаний в amoCRM
/// </summary>
public sealed class AmoCrmCompanyService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmCompanyService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmCompanyService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmCompanyService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await GetCompaniesAsync(accessToken, subdomain, [], query, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Company>> GetCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка компаний из аккаунта {Subdomain}. Поиск по вхождению {Query}", subdomain, query);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCompanies(subdomain);
        Uri requestUri = AddSearchQueryParameter(uriBuilder.Uri, query);
        requestUri = AddLinkedEntitiesParameters(requestUri, linkedEntityTypes);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            requestUri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<Company> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Companies ?? [],
            subdomain,
            OperationDescriptions.GetCompanies,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Компании из аккаунта {Subdomain} загружены успешно. Получено {CompaniesCount} компаний", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Company>> AddCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Добавление компаний в аккаунт {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCompanies(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddCompanyRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Company> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Companies ?? [],
            subdomain,
            OperationDescriptions.AddCompanies,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление компаний в аккаунт {Subdomain} успешно завершено. Добавлено {AddedCompaniesCount} компаний из {RequestedCompaniesCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Company>> UpdateCompaniesAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateCompanyRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Редактирование компаний в аккаунте {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCompanies(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateCompanyRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        var response =  await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Companies ?? [],
            subdomain,
            OperationDescriptions.UpdateCompanies,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Редактирование компаний в аккаунте {Subdomain} успешно завершено. Обновлено {UpdatedCompaniesCount} компаний из {RequestedCompaniesCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }
}

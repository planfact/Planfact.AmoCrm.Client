using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Contacts;

/// <summary>
/// Сервис контактов в amoCRM
/// </summary>
public sealed class AmoCrmContactService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmContactService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmContactService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmContactService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        return await GetContactsAsync(accessToken, subdomain, [], query, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка контактов из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);
        Uri requestUri = AddSearchQueryParameter(uriBuilder.Uri, query);
        requestUri = AddLinkedEntitiesParameters(requestUri, linkedEntityTypes);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            requestUri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<Contact> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Contacts ?? [],
            subdomain,
            OperationDescriptions.GetContacts,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Контакты из аккаунта {Subdomain} загружены успешно. Получено {ContactsCount} контактов", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        CancellationToken cancellationToken = default)
    {
        return await GetContactByIdAsync(accessToken, subdomain, contactId, [], cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Contact> GetContactByIdAsync(
        string accessToken,
        string subdomain,
        int contactId,
        IReadOnlyCollection<EntityType> linkedEntityTypes,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Поиск контакта с ID {ContactId} в аккаунте {Subdomain}", contactId, subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain, contactId);
        Uri requestUri = AddLinkedEntitiesParameters(uriBuilder.Uri, linkedEntityTypes);
        IDictionary<string, string> headers = GetDefaultHeaders(accessToken);

        Contact response = await HttpClient.GetAsync<Contact>(
            requestUri,
            headers,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Поиск контакта с ID {ContactId} в аккаунте {Subdomain} завершен. Найден контакт {ContactName}", contactId, subdomain, response.Name);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Contact>> AddContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Добавление контактов в аккаунт {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddContactRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Contact> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Contacts ?? [],
            subdomain,
            OperationDescriptions.AddContacts,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление контактов в аккаунт {Subdomain} успешно завершено. Добавлено {AddedContactsCount} контактов из {RequestedContactsCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Contact>> UpdateContactsAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateContactRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Редактирование контактов в аккаунте {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateContactRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Contact> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Contacts ?? [],
            subdomain,
            OperationDescriptions.UpdateContacts,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Редактирование контактов в аккаунте {Subdomain} успешно завершено. Обновлено {UpdatedContactsCount} контактов из {RequestedContactsCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }
}

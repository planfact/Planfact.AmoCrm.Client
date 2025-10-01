using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Contacts;

/// <summary>
/// Сервис контактов в amoCRM
/// </summary>
public sealed class AmoCrmContactService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmContactService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmContactService
{
    private readonly IHttpClientAdapter _httpClient = httpClient;
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmContactService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Contact>> GetContactsAsync(
        string accessToken,
        string subdomain,
        string query = "",
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка контактов из аккаунта {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);
        Uri requestUri = AddSearchQueryParameter(uriBuilder.Uri, query);

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
        _logger.LogDebug("Поиск контакта с ID {ContactId} в аккаунте {Subdomain}", contactId, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain, contactId);
        IDictionary<string, string> headers = GetDefaultHeaders(accessToken);

        Contact response = await _httpClient.GetAsync<Contact>(
            uriBuilder.Uri,
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
        ArgumentNullException.ThrowIfNull(requests);

        if (requests!.Count == 0)
        {
            return [];
        }

        _logger.LogDebug("Добавление контактов в аккаунт {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddContactRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Contact> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Contacts ?? throw new AmoCrmHttpException("Получен null ответ от API"),
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
        ArgumentNullException.ThrowIfNull(requests);

        if (requests!.Count == 0)
        {
            return [];
        }

        _logger.LogDebug("Редактирование контактов в аккаунте {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForContacts(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateContactRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Contact> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Contacts ?? throw new AmoCrmHttpException("Получен null ответ от API"),
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

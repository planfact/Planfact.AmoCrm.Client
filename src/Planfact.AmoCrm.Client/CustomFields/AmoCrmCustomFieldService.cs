using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Сервис дополнительных полей в amoCRM
/// </summary>
public sealed class AmoCrmCustomFieldService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmCustomFieldService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmCustomFieldService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmCustomFieldService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<CustomField>> GetCustomFieldsAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);

        _logger.LogDebug("Загрузка дополнительных полей из аккаунта {Subdomain}. Тип сущности {EntityType}", subdomain, entityTypeName);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForCustomFields(subdomain, entityTypeName);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<CustomField> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.CustomFields ?? [],
            subdomain,
            OperationDescriptions.GetCustomFields,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Загрузка дополнительных полей из аккаунта {Subdomain} успешно завершена. Тип сущности {EntityType}", subdomain, entityTypeName);

        return response;
    }
}

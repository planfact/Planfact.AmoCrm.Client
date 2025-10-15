using Planfact.AmoCrm.Client.Common;

using Reliable.HttpClient;

namespace Planfact.AmoCrm.Client.Links;

/// <summary>
/// Сервис связей сущностей в amoCRM
/// </summary>
public sealed class AmoCrmLinkService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmLinkService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmLinkService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmLinkService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EntityLink>> GetLinksAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        EntityLinksFilter filter,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);

        _logger.LogDebug("Загрузка связей сущностей из аккаунта {Subdomain}. Тип главной сущности {EntityType}", subdomain, entityTypeName);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(filter);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForLinks(subdomain, entityTypeName);

        var filterQuery = BuildFilterQuery(filter);

        uriBuilder.Query = string.Concat(uriBuilder.Query, filterQuery);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<EntityLink> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Links ?? [],
            subdomain,
            OperationDescriptions.GetLinks,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Связи сущностей из аккаунта {Subdomain} успешно загружены. Тип главной сущности {EntityType}. Получено {LinksCount} связей",
            subdomain,
            entityTypeName,
            response.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EntityLink>> LinkAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<LinkEntitiesRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);

        _logger.LogDebug("Создание связей сущностей в аккаунте {Subdomain}. Тип главной сущности {EntityType}", subdomain, entityTypeName);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForAddLinks(subdomain, entityTypeName);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<LinkEntitiesRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<EntityLink> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Links ?? [],
            subdomain,
            OperationDescriptions.AddLinks,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Создание связей сущностей в аккаунте {Subdomain} успешно завершено. Тип главной сущности {EntityType}. Создано {AddedLinksCount} связей из {RequestedLinksCount}",
            subdomain,
            entityTypeName,
            requests.Count,
            response.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task UnlinkAsync(
        string accessToken,
        string subdomain,
        EntityType entityType,
        IReadOnlyCollection<UnlinkEntitiesRequest> requests,
        CancellationToken cancellationToken = default)
    {
        var entityTypeName = EntityTypeConverter.ToString(entityType);

        _logger.LogDebug("Удаление связей сущностей в аккаунте {Subdomain}. Тип главной сущности {EntityType}", subdomain, entityTypeName);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return;
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForDeleteLinks(subdomain, entityTypeName);

        IAsyncEnumerable<HttpResponseMessage> batchTask = PostInBatchesAsync(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        var errorsCount = 0;
        await foreach (HttpResponseMessage response in batchTask.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (response.IsSuccessStatusCode)
            {
                continue;
            }

            errorsCount++;
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogWarning(
                "Ошибка при удалении связей сущностей. Статус: {StatusCode}. Содержимое ответа: {ResponseContent}",
                response.StatusCode,
                responseContent
            );
        }

        _logger.LogDebug(
            "Удаление связей сущностей в аккаунте {Subdomain} завершено. Тип главной сущности {EntityType}. Кол-во ошибок: {ErrorsCount}",
            subdomain,
            entityTypeName,
            errorsCount
        );
    }

    private static string BuildFilterQuery(EntityLinksFilter filter)
    {
        IEnumerable<string> filterEntityIdQueryParameters = filter.EntityIds
            .Select((id, index) => $"filter[entity_id][{index}]={id}");

        var filterEntityIdQuery = string.Join('&', filterEntityIdQueryParameters);
        var filterLinkedEntityIdQuery = string.Empty;
        var filterLinkedEntityTypeQuery = string.Empty;
        var filterCatalogIdQuery = string.Empty;

        if (filter.LinkedEntityId.HasValue && filter.LinkedEntityType.HasValue)
        {
            filterLinkedEntityIdQuery = $"&filter[to_entity_id]={filter.LinkedEntityId}";
            filterLinkedEntityTypeQuery = $"&filter[to_entity_type]={EntityTypeConverter.ToString(filter.LinkedEntityType.Value)}";
        }

        if (filter.CatalogId.HasValue)
        {
            filterCatalogIdQuery = $"&filter[to_catalog_id]={filter.CatalogId}";
        }

        return string.Concat(
            filterEntityIdQuery,
            filterLinkedEntityIdQuery,
            filterLinkedEntityTypeQuery,
            filterCatalogIdQuery
        );
    }
}

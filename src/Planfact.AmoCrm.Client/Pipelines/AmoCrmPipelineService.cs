using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Сервис воронок сделок в amoCRM
/// </summary>
public sealed class AmoCrmPipelineService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmPipelineService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmPipelineService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmPipelineService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка статусов сделок из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForPipelines(subdomain);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<LeadStatus> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Pipelines?
                .SelectMany(p => p.AvailableStatuses?.Statuses ?? [])
                .DistinctBy(s => s.Id) ?? [],
            subdomain,
            OperationDescriptions.GetLeadStatuses,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Загрузка статусов сделок из аккаунта {Subdomain} успешно завершена. Загружено {StatusesCount} статусов", subdomain, response.Count);

        return response;
    }
}

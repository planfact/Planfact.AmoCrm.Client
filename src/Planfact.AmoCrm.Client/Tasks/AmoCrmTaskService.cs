using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;

using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Сервис задач в amoCRM
/// </summary>
public sealed class AmoCrmTaskService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmTaskService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmTaskService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmTaskService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка задач из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IReadOnlyCollection<AmoCrmTask> response = await GetTasksCoreAsync(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Задачи из аккаунта {Subdomain} загружены успешно. Получено {TasksCount} задач", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        TasksFilter filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка задач из аккаунта {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(filter);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        var filterQuery = BuildFilterQuery(filter);

        uriBuilder.Query = string.Concat(uriBuilder.Query, filterQuery);

        IReadOnlyCollection<AmoCrmTask> response = await GetTasksCoreAsync(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Задачи из аккаунта {Subdomain} загружены успешно. Получено {TasksCount} задач", subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Добавление задач в аккаунт {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddTaskRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<AmoCrmTask> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Tasks ?? [],
            subdomain,
            OperationDescriptions.AddTasks,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление задач в аккаунт {Subdomain} успешно завершено. Добавлено {AddedTasksCount} задач из {RequestedTasksCount}",
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Редактирование задач в аккаунте {Subdomain}", subdomain);

        ValidateCredentials(accessToken, subdomain);

        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateTaskRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<AmoCrmTask> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Tasks ?? [],
            subdomain,
            OperationDescriptions.UpdateTasks,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
           "Редактирование задач в аккаунте {Subdomain} успешно завершено. Обновлено {UpdatedTasksCount} задач из {RequestedTasksCount}",
           subdomain,
           response.Count,
           requests.Count
       );

        return response;
    }

    private async Task<IReadOnlyCollection<AmoCrmTask>> GetTasksCoreAsync(
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
            r => r.Embedded?.Tasks ?? [],
            subdomain,
            OperationDescriptions.GetTasks,
            cancellationToken
        ).ConfigureAwait(false);
    }

    private static string BuildFilterQuery(TasksFilter filter)
    {
        IEnumerable<string> filterTaskIdQueryParameters = filter.TaskIds
            .Select((id, index) => $"filter[id][{index}]={id}");
        IEnumerable<string> filterTaskTypeIdQueryParameters = filter.TaskTypeIds
            .Select((id, index) => $"filter[task_type][{index}]={id}");
        IEnumerable<string> filterResponsibleUserIdQueryParameters = filter.ResponsibleUserIds
            .Select((id, index) => $"filter[responsible_user_id][{index}]={id}");

        var filterTaskIdQuery = string.Join('&', filterTaskIdQueryParameters);
        var filterTaskTypeIdIdQuery = string.Join('&', filterTaskTypeIdQueryParameters);
        var filterResponsibleUserIdQuery = string.Join('&', filterResponsibleUserIdQueryParameters);
        var filterEntityIdQuery = string.Empty;
        var filterEntityTypeQuery = string.Empty;
        var updatedAtFromQuery = string.Empty;
        var updatedAtToQuery = string.Empty;
        var isCompletedQuery = string.Empty;

        if (filter.EntityType.HasValue)
        {
            IEnumerable<string> filterEntityIdQueryParameters = filter.EntityIds
                .Select((id, index) => $"filter[entity_id][{index}]={id}");

            filterEntityIdQuery = string.Join('&', filterEntityIdQueryParameters);
            filterEntityTypeQuery = $"filter[entity_type]={EntityTypeConverter.ToString(filter.EntityType.Value)}";
        }

        if (filter.UpdatedAtFrom.HasValue)
        {
            updatedAtFromQuery = $"filter[updated_at][from]={filter.UpdatedAtFrom}";
        }

        if (filter.UpdatedAtTo.HasValue)
        {
            updatedAtToQuery = $"filter[updated_at][to]={filter.UpdatedAtTo}";
        }

        if (filter.IsCompleted.HasValue)
        {
            isCompletedQuery = $"filter[is_completed]={Convert.ToByte(filter.IsCompleted.Value)}";
        }

        string[] filterQueryParameters =
        [
            filterTaskIdQuery,
            filterTaskTypeIdIdQuery,
            filterResponsibleUserIdQuery,
            filterEntityIdQuery,
            filterEntityTypeQuery,
            updatedAtFromQuery,
            updatedAtToQuery,
            isCompletedQuery
        ];

        return string.Join(
            '&',
            filterQueryParameters.Where(p => !string.IsNullOrEmpty(p))
        );
    }
}

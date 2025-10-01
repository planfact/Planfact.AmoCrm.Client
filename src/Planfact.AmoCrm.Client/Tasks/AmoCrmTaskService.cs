using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Exceptions;

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

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<AmoCrmTask> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Tasks ?? [],
            subdomain,
            OperationDescriptions.GetTasks,
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
        ArgumentNullException.ThrowIfNull(requests);

        if (requests!.Count == 0)
        {
            return [];
        }

        _logger.LogDebug("Добавление задач в аккаунт {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddTaskRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<AmoCrmTask> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Tasks ?? throw new AmoCrmHttpException("Получен null ответ от API"),
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
        ArgumentNullException.ThrowIfNull(requests);

        if (requests!.Count == 0)
        {
            return [];
        }

        _logger.LogDebug("Редактирование задач в аккаунте {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTasks(subdomain);

        IAsyncEnumerable<EntitiesResponse> batchTask = PatchInBatchesAsync<UpdateTaskRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<AmoCrmTask> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Tasks ?? throw new AmoCrmHttpException("Получен null ответ от API"),
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
}

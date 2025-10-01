using AmoCrmTask = Planfact.AmoCrm.Client.Tasks.Task;

namespace Planfact.AmoCrm.Client.Tasks;

/// <summary>
/// Контракт сервиса задач в amoCRM
/// </summary>
public interface IAmoCrmTaskService
{
    /// <summary>
    /// Получение списка задач из аккаунта в amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих детальную информацию о найденных компаниях</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> GetTasksAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание задач в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на создание задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о созданных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> AddTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<AddTaskRequest> requests,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Редактирование задач в amoCRM с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="requests">Коллекция запросов на редактирование задач</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию об обновленных задачах</returns>
    public Task<IReadOnlyCollection<AmoCrmTask>> UpdateTasksAsync(
        string accessToken,
        string subdomain,
        IReadOnlyCollection<UpdateTaskRequest> requests,
        CancellationToken cancellationToken = default);
}

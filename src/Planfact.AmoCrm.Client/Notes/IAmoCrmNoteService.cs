
namespace Planfact.AmoCrm.Client.Notes;

/// <summary>
/// Контракт сервиса примечаний в amoCRM
/// </summary>
public interface IAmoCrmNoteService
{
    /// <summary>
    /// Получение примечаний по заданным фильтрам
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="noteType">Тип примечания amoCRM</param>
    /// <param name="entityId">Идентификатор сущности amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список примечаний. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Note>> GetNotesAsync(
        string accessToken,
        string subdomain,
        Common.EntityType entityType,
        AmoCrmNoteTypeEnum noteType,
        int? entityId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание примечаний с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности amoCRM</param>
    /// <param name="requests">Коллекция запросов на добавление примечаний</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных примечаний</returns>
    public Task<IReadOnlyCollection<Note>> AddNotesAsync(
        string accessToken,
        string subdomain,
        Common.EntityType entityType,
        IReadOnlyCollection<AddNoteRequest> requests,
        CancellationToken cancellationToken = default);
}

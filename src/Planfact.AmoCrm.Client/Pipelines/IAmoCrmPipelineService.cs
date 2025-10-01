
namespace Planfact.AmoCrm.Client.Pipelines;

/// <summary>
/// Контракт сервиса воронок сделок в amoCRM
/// </summary>
public interface IAmoCrmPipelineService
{
    /// <summary>
    /// Получение доступных статусов сделок
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список статусов сделок. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<LeadStatus>> GetLeadStatusesAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);
}

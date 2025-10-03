
namespace Planfact.AmoCrm.Client.CustomFields;

/// <summary>
/// Контракт сервиса дополнительных полей в amoCRM
/// </summary>
public interface IAmoCrmCustomFieldService
{
    /// <summary>
    /// Получение дополнительных полей, настроенных в аккаунте
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="entityType">Тип сущности в amoCRM, поля которой необходимо получить</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список дополнительных полей. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<CustomField>> GetCustomFieldsAsync(
        string accessToken,
        string subdomain,
        Common.EntityType entityType,
        CancellationToken cancellationToken = default);
}

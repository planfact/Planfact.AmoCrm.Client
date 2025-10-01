
namespace Planfact.AmoCrm.Client.Users;

/// <summary>
/// Контракт сервиса пользователей в amoCRM
/// </summary>
public interface IAmoCrmUserService
{
    /// <summary>
    /// Получение списка пользователей, привязанных к учетной записи amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция объектов, содержащих информацию о пользователях аккаунта. Если ничего не найдено, возвращает пустую коллекцию</returns>
    public Task<IReadOnlyCollection<User>> GetUsersAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);
}

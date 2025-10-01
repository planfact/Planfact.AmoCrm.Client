namespace Planfact.AmoCrm.Client.Account;

/// <summary>
/// Контракт сервиса для работы с аккаунтом и виджетами amoCRM
/// </summary>
public interface IAmoCrmAccountService
{
    /// <summary>
    /// Получение информации об аккаунте amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа для API amoCRM</param>
    /// <param name="subdomain">Поддомен аккаунта amoCRM</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация об аккаунте</returns>
    Task<AccountResponse> GetAccountAsync(
        string accessToken,
        string subdomain,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение информации о виджете amoCRM
    /// </summary>
    /// <param name="accessToken">Токен доступа для API amoCRM</param>
    /// <param name="subdomain">Поддомен аккаунта amoCRM</param>
    /// <param name="widgetCode">Код виджета</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Информация о виджете</returns>
    Task<WidgetResponse> GetWidgetAsync(
        string accessToken,
        string subdomain,
        string widgetCode,
        CancellationToken cancellationToken = default);
}

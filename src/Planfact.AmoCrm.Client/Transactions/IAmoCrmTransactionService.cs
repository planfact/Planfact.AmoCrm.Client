
namespace Planfact.AmoCrm.Client.Transactions;

/// <summary>
/// Контракт сервиса транзакций в amoCRM
/// </summary>
public interface IAmoCrmTransactionService
{
    /// <summary>
    /// Получение списка транзакций покупателя
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список транзакций. Возвращает пустой список, если ничего не найдено</returns>
    public Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создание транзакций, привязанных к покупателю, с поддержкой пакетной обработки
    /// </summary>
    /// <param name="accessToken">Токен доступа к API amoCRM</param>
    /// <param name="subdomain">Поддомен учетной записи amoCRM</param>
    /// <param name="customerId">Идентификатор покупателя</param>
    /// <param name="requests">Коллекция запросов на добавление транзакций</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список созданных транзакций</returns>
    public Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default);
}

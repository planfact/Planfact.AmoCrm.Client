using Reliable.HttpClient;

using Planfact.AmoCrm.Client.Common;
using Planfact.AmoCrm.Client.Exceptions;

namespace Planfact.AmoCrm.Client.Transactions;

/// <summary>
/// Сервис транзакций в amoCRM
/// </summary>
public sealed class AmoCrmTransactionService(
    IHttpClientAdapter httpClient,
    AmoCrmUriBuilderFactory uriBuilderFactory,
    ILogger<AmoCrmTransactionService> logger) : AmoCrmServiceBase(httpClient, logger), IAmoCrmTransactionService
{
    private readonly AmoCrmUriBuilderFactory _uriBuilderFactory = uriBuilderFactory;
    private readonly ILogger<AmoCrmTransactionService> _logger = logger;

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Transaction>> GetTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Загрузка транзакций покупателя {CustomerId} из аккаунта {Subdomain}", customerId, subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTransactions(subdomain, customerId);

        IAsyncEnumerable<EntitiesResponse> paginationTask = GetPaginatedAsync<EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            cancellationToken
        );

        IReadOnlyCollection<Transaction> response = await CollectPaginatedEntitiesAsync(
            paginationTask,
            r => r.Embedded?.Transactions ?? [],
            subdomain,
            OperationDescriptions.GetTransactions,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug("Транзакции покупателя {CustomerId} из аккаунта {Subdomain} успешно загружены. Получено {TransactionsCount} транзакций", customerId, subdomain, response.Count);

        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Transaction>> AddTransactionsAsync(
        string accessToken,
        string subdomain,
        int customerId,
        IReadOnlyCollection<AddTransactionRequest> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (requests!.Count == 0)
        {
            return [];
        }

        _logger.LogDebug("Добавление транзакций в аккаунт {Subdomain}", subdomain);

        UriBuilder uriBuilder = _uriBuilderFactory.CreateForTransactions(subdomain, customerId);

        IAsyncEnumerable<EntitiesResponse> batchTask = PostInBatchesAsync<AddTransactionRequest, EntitiesResponse>(
            uriBuilder.Uri,
            accessToken,
            requests,
            cancellationToken
        );

        IReadOnlyCollection<Transaction> response = await CollectPaginatedEntitiesAsync(
            batchTask,
            r => r.Embedded?.Transactions ?? throw new AmoCrmHttpException("Получен null ответ от API"),
            subdomain,
            OperationDescriptions.AddTransactions,
            cancellationToken
        ).ConfigureAwait(false);

        _logger.LogDebug(
            "Добавление транзакций покупателя {CustomerId} в аккаунт {Subdomain} успешно завершено. Добавлено {AddedTransactionsCount} транзакций из {RequestedTransactionsCount}",
            customerId,
            subdomain,
            response.Count,
            requests.Count
        );

        return response;
    }
}
